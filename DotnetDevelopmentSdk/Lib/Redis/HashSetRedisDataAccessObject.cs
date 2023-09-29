// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DotnetDevelopmentSdk.Lib.Redis;

public abstract class
    HashSetRedisDataAccessObject<TData, TRecord, TKeyProvider, TEntryKeyProvider> : BaseRedisDataAccessObject
    where TRecord : GenericRedisRecord<TData, TEntryKeyProvider>
    where TKeyProvider : CompositeKeyProvider
    where TEntryKeyProvider : CompositeKeyProvider
{
    protected HashSetRedisDataAccessObject(IRedisDatabase redisDatabase) : base(redisDatabase)
    {
    }

    public async Task SaveManyAsync(TKeyProvider keyProvider, List<TRecord> records,
        ITransaction transaction = null,
        bool submitTx = true)
    {
        await UseTransaction(transaction,
            new List<Func<IDatabaseAsync, Task>>
            {
                db => db.HashSetAsync(keyProvider.GetKey(),
                    records.Where(record => record.Value != null).Select(record =>
                            new HashEntry(record.MemberKeyProvider.GetKey(), JsonConvert.SerializeObject(record.Value)))
                        .ToArray()),
                db => db.KeyExpireAsync(keyProvider.GetKey(), ExpirationTime)
            }, submitTx: submitTx);
    }

    public async Task SaveAsync(TKeyProvider keyProvider, TRecord record, ITransaction transaction = null,
        bool submitTx = true)
    {
        await SaveManyAsync(keyProvider, new List<TRecord>() { record }, transaction, submitTx);
    }

    public async Task<Dictionary<string, TRecord>> GetManyAsync(TKeyProvider keyProvider,
        List<TEntryKeyProvider> memberKeyProviders)
    {
        var hashEntries = await Database.HashGetAsync(keyProvider.GetKey(),
            memberKeyProviders.Select(memberKeyProvider => new RedisValue(memberKeyProvider.GetKey())).ToArray());
        return memberKeyProviders.Zip(hashEntries)
            .Where(keyProviderAndHashEntry => keyProviderAndHashEntry.Second.HasValue).ToDictionary(
                keyProviderAndHashEntry => keyProviderAndHashEntry.First.GetKey(),
                keyProviderAndHashEntry => ToRecord(keyProviderAndHashEntry.First,
                    new HashEntry(keyProviderAndHashEntry.First.GetKey(), keyProviderAndHashEntry.Second)));
    }

    public async Task<TRecord> GetAsync(TKeyProvider keyProvider, TEntryKeyProvider memberKeyProvider)
    {
        var redisValue = await Database.HashGetAsync(keyProvider.GetKey(), memberKeyProvider.GetKey());
        return ToRecord(memberKeyProvider,
            redisValue.HasValue
                ? new HashEntry(memberKeyProvider.GetKey(), redisValue)
                : new HashEntry(memberKeyProvider.GetKey(), new RedisValue(null)));
    }

    public async Task<List<TRecord>> GetAllAsync(TKeyProvider keyProvider)
    {
        var hashEntries = await Database.HashGetAllAsync(keyProvider.GetKey());
        return hashEntries.Where(hashEntry => hashEntry.Value.HasValue).Select(hashEntry =>
        {
            var memberKeyProvider =
                (TEntryKeyProvider)Activator.CreateInstance(typeof(TEntryKeyProvider))!;
            memberKeyProvider.SetKey(hashEntry.Name);
            return ToRecord(memberKeyProvider, hashEntry);
        }).ToList();
    }

    private static TRecord ToRecord(TEntryKeyProvider keyProvider, HashEntry hashEntry)
    {
        return (TRecord)Activator.CreateInstance(typeof(TRecord), keyProvider,
            hashEntry.Value.HasValue ? JsonConvert.DeserializeObject<TData>(hashEntry.Value) : null);
    }
}
