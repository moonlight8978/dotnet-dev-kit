// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DotnetDevelopmentSdk.Lib.Redis.SortedSet;

public interface ISortedSetMemberKey
{
    public double Prepare();

    public void Consume(double score);
}

public class RedisSortedSetMemberRecord<TKey, TData> where TKey : ISortedSetMemberKey where TData : IRedisData
{
    public TKey Key { get; set; }

    public TData? Data { get; set; }

    public bool HasValue => Data != null;

    public RedisSortedSetMemberRecord(TKey key, TData? data)
    {
        Key = key;
        Data = data;
    }
}

public abstract class SortedSetRedisDao<TRootKey, TMemberKey, TData> : ITypeDirectedScopeBindedService
    where TRootKey : IRedisKey where TMemberKey : ISortedSetMemberKey, new() where TData : IRedisData, new()
{
    private readonly IDatabase _database;

    public class MemberRecord : RedisSortedSetMemberRecord<TMemberKey, TData>
    {
        public MemberRecord(TMemberKey key, TData? data) : base(key, data)
        {
        }
    }

    public class Record : RedisRecord<TRootKey, List<MemberRecord>>
    {
        public Record(TRootKey key, List<MemberRecord> data) : base(key, data)
        {
        }
    }

    protected SortedSetRedisDao(IRedisDatabase redisDatabase)
    {
        _database = redisDatabase.Database;
    }

    public async Task SaveAsync(TRootKey rootKey, TMemberKey memberKey, TData memberData)
    {
        await SaveAsync(rootKey, new[] { new MemberRecord(memberKey, memberData) });
    }

    public async Task SaveAsync(TRootKey rootKey, IEnumerable<MemberRecord> members)
    {
        var key = rootKey.Prepare();

        var pairs = members.Where(record => record.HasValue)
            .Select(record => new SortedSetEntry(record.Data!.Prepare(), record.Key.Prepare()))
            .ToArray();

        await _database.SortedSetAddAsync(key, pairs);

        var ttl = rootKey.Ttl;
        if (ttl != null)
        {
            await _database.KeyExpireAsync(key, ttl);
        }
    }

    public async Task<Record> GetManyByScoreAsync(TRootKey rootKey, double min, double max)
    {
        var key = rootKey.Prepare();
        var result = await _database.SortedSetRangeByScoreWithScoresAsync(key, min, max);
        var records = result
            .Where(entry => entry.Element.HasValue)
            .Select(entry =>
            {
                var memberKey = new TMemberKey();
                memberKey.Consume(entry.Score);
                var memberData = new TData();
                memberData.Consume(entry.Element!);
                return new MemberRecord(memberKey, memberData);
            }).ToList();
        return new Record(rootKey, records);
    }
}
