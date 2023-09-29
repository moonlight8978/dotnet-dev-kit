// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DotnetDevelopmentSdk.Lib.Redis;

public abstract class StringRedisDataAccessObject<TData, TRecord, TKeyProvider> : BaseRedisDataAccessObject
    where TRecord : GenericRedisRecord<TData, TKeyProvider> where TKeyProvider : CompositeKeyProvider
{
    protected StringRedisDataAccessObject(IRedisDatabase redisDatabase) : base(redisDatabase)
    {
    }

    public async Task SaveManyAsync(List<TRecord> records, ITransaction transaction = null, bool submitTx = true)
    {
        // TODO: Support non-expire items
        var expires =
            records.Select<TRecord, Func<IDatabaseAsync, Task>>(record =>
            {
                Task Expire(IDatabaseAsync db) =>
                    db.KeyExpireAsync(record.MemberKeyProvider.GetKey(), ExpirationTime);

                return Expire;
            });

        await UseTransaction(transaction,
            new List<Func<IDatabaseAsync, Task>>
            {
                db => db.StringSetAsync(records.Select(record =>
                    new KeyValuePair<RedisKey, RedisValue>(record.MemberKeyProvider.GetKey(),
                        record.Value == null ? null : JsonConvert.SerializeObject(record.Value))).ToArray()),
            }.Concat(expires).ToList(), submitTx: submitTx);
    }

    public async Task RemoveAsync(TKeyProvider keyProvider, ITransaction transaction = null,
        bool submitTx = true)
    {
        await UseTransaction(transaction,
            new List<Func<IDatabaseAsync, Task>> { db => db.KeyDeleteAsync(keyProvider.GetKey()) }, submitTx: submitTx);
    }

    public async Task SaveAsync(TRecord record, ITransaction transaction = null, bool submitTx = true)
    {
        await SaveManyAsync(new List<TRecord> { record }, transaction, submitTx);
    }

    public async Task<TRecord> GetAsync(TKeyProvider keyProvider)
    {
        var result = await Database.StringGetAsync(keyProvider.GetKey());
        return ToRecord(keyProvider, result);
    }

    public async Task<Dictionary<string, TRecord>> GetManyAsync(List<TKeyProvider> keyProviders)
    {
        var redisValues =
            await Database.StringGetAsync(keyProviders.Select(keyProvider => new RedisKey(keyProvider.GetKey()))
                .ToArray());
        return keyProviders.Zip(redisValues).Where(keyProviderAndRedisValue => keyProviderAndRedisValue.Second.HasValue)
            .ToDictionary(keyProviderAndRedisValue => keyProviderAndRedisValue.First.GetKey(),
                keyProviderAndRedisValue => ToRecord(keyProviderAndRedisValue.First, keyProviderAndRedisValue.Second));
    }

    private static TRecord ToRecord(TKeyProvider keyProvider, RedisValue redisValue)
    {
        var data = redisValue.HasValue ? JsonConvert.DeserializeObject<TData>(redisValue) : default;
        return (TRecord)Activator.CreateInstance(typeof(TRecord), keyProvider, data)!;
    }
}
