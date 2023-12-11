// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DotnetDevelopmentSdk.Lib.Redis.SortedSet;

public abstract class StringRedisDao<TKey, TData> : ITypeDirectedScopeBindedService
    where TKey : IRedisKey where TData : IRedisData, new()
{
    private readonly IDatabase _database;

    public class Record : RedisRecord<TKey, TData>
    {
        public Record(TKey key, TData? data) : base(key, data)
        {
        }
    }

    protected StringRedisDao(IRedisDatabase redisDatabase)
    {
        _database = redisDatabase.Database;
    }

    public async Task SaveAsync(TKey key, TData data)
    {
        await SaveAsync(new Record(key, data));
    }

    public async Task SaveAsync(Record record)
    {
        await SaveManyAsync(new List<Record> { record });
    }

    public async Task<Record> GetAsync(TKey key)
    {
        var result = await _database.StringGetAsync(key.Prepare());
        if (!result.HasValue)
        {
            return new Record(key, default);
        }

        var data = new TData();
        data.Consume(result!);
        return new Record(key, data);
    }

    public async Task SaveManyAsync(List<Record> records)
    {
        var pairs = records
            .Where(record => record.HasValue)
            .Select(record => new KeyValuePair<RedisKey, RedisValue>(record.Key.Prepare(), record.Data!.Prepare()))
            .ToArray();

        await _database.StringSetAsync(pairs);

        foreach (var record in records)
        {
            var ttl = record.Key.Ttl;
            if (ttl == null || ttl == TimeSpan.Zero)
            {
                continue;
            }

            await _database.KeyExpireAsync(record.Key.Prepare(), ttl);
        }
    }
}
