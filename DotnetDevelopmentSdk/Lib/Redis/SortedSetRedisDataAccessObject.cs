// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DotnetDevelopmentSdk.Lib.Redis;

public abstract class SortedSetEntryRedisRecord<TKeyProvider> : BaseRedisRecord
    where TKeyProvider : CompositeKeyProvider
{
    public abstract void FromEntry(SortedSetEntry sortedSetEntry);

    public abstract double Score { get; }

    protected SortedSetEntryRedisRecord() : this(null) { }

    protected SortedSetEntryRedisRecord(TKeyProvider keyProvider) : base(keyProvider)
    {
    }

    protected TKeyProvider CreateKeyProvider(SortedSetEntry sortedSetEntry)
    {
        return CreateKeyProvider(sortedSetEntry.Element);
    }

    protected TKeyProvider CreateKeyProvider(string key)
    {
        var keyProvider = (TKeyProvider)Activator.CreateInstance(typeof(TKeyProvider))!;
        keyProvider.SetKey(key);
        return keyProvider;
    }
}

public abstract class
    SortedSetRedisDataAccessObject<TRecord, TKeyProvider, TEntryKeyProvider> : BaseRedisDataAccessObject
    where TRecord : SortedSetEntryRedisRecord<TEntryKeyProvider>
    where TEntryKeyProvider : CompositeKeyProvider
    where TKeyProvider : CompositeKeyProvider
{
    protected SortedSetRedisDataAccessObject(IRedisDatabase redisDatabase) : base(redisDatabase)
    {
    }

    public Task DeleteAsync(TKeyProvider rootKeyProvider, ITransaction transaction = null, bool submitTx = true)
    {
        var key = rootKeyProvider.GetKey();
        return UseTransaction(transaction,
            new List<Func<IDatabaseAsync, Task>>
            {
                db => db.KeyDeleteAsync(key)
            }, submitTx: submitTx);
    }

    public Task SaveManyAsync(TKeyProvider keyProvider, IEnumerable<TRecord> records,
        ITransaction transaction = null, bool submitTx = true)
    {
        var key = keyProvider.GetKey();
        return UseTransaction(transaction,
            new List<Func<IDatabaseAsync, Task>>
            {
                db => db.SortedSetAddAsync(key,
                    records.Select(record => new SortedSetEntry(record.MemberKeyProvider.GetKey(), record.Score))
                        .ToArray()),
                db => db.KeyExpireAsync(key, ExpirationTime)
            }, submitTx: submitTx);
    }

    public async Task<TRecord?> GetByPositionAsync(TKeyProvider keyProvider, int position, Order direction)
    {
        if (position < 1)
        {
            throw new ArgumentException("Position cannot be less than 1");
        }

        var key = keyProvider.GetKey();

        var queryResults =
            await Database.SortedSetRangeByRankWithScoresAsync(key, position - 1,
                position - 1, direction);
        var result = queryResults.FirstOrDefault();
        if (result == default)
        {
            return null;
        }

        return ToSortedSetRecord(result);
    }

    public async Task<List<TRecord>> GetAllByPositionAsync(TKeyProvider keyProvider, int position,
        Order orderDirection)
    {
        if (position < 1)
        {
            throw new ArgumentException("Position cannot be less than 1");
        }

        return await GetAllByPositionAsync(keyProvider, position - 1, position + 1, orderDirection);
    }

    public async Task<List<TRecord>> GetAllByPositionAsync(TKeyProvider keyProvider, int positionStart, int positionEnd,
        Order orderDirection)
    {
        var key = keyProvider.GetKey();
        var queryResults =
            await Database.SortedSetRangeByRankWithScoresAsync(key, positionStart, positionEnd, orderDirection);

        return queryResults
            .Select(ToSortedSetRecord)
            .ToList();
    }

    public async Task<List<TRecord>> GetAllByScoreAsync(TKeyProvider keyProvider, double minScore,
        double maxScore, Order orderDirection = Order.Ascending)
    {
        var results =
            await Database.SortedSetRangeByScoreWithScoresAsync(keyProvider.GetKey(), minScore, maxScore,
                order: orderDirection);

        return results == null ? new List<TRecord>() : results.Select(ToSortedSetRecord).ToList();
    }

    public async Task<double?> GetByIdAsync(TKeyProvider keyProvider,
        TEntryKeyProvider entryKeyProvider)
    {
        var entryKey = entryKeyProvider.GetKey();
        var score = await Database.SortedSetScoreAsync(keyProvider.GetKey(), entryKey);

        if (!score.HasValue)
        {
            return null;
        }

        return score.Value;
    }

    public async Task<Dictionary<string, double>> GetManyByIdAsync(TKeyProvider keyProvider,
        IEnumerable<TEntryKeyProvider> entryKeyProviders)
    {
        var idToScore = new Dictionary<string, double>();

        foreach (var entryKeyProvider in entryKeyProviders)
        {
            var entryKey = entryKeyProvider.GetKey();
            var score = await Database.SortedSetScoreAsync(keyProvider.GetKey(), entryKey);
            if (!score.HasValue)
            {
                continue;
            }

            idToScore.Add(entryKey, score.Value);
        }

        return idToScore;
    }

    public async Task<int> GetRankAsync(TKeyProvider keyProvider, TEntryKeyProvider entryKeyProvider,
        Func<List<TRecord>, Task<List<TRecord>>> sortAsync,
        Order orderDirection = Order.Descending)
    {
        var id = entryKeyProvider.GetKey();
        var score = await Database.SortedSetScoreAsync(keyProvider.GetKey(), id);

        if (!score.HasValue)
        {
            return -1;
        }

        var entriesHasSameScore =
            await Database.SortedSetRangeByScoreWithScoresAsync(keyProvider.GetKey(), score.Value,
                score.Value, order: orderDirection);

        var records = entriesHasSameScore.Select(ToSortedSetRecord).ToList();

        if (records.Count == 0)
        {
            return -1;
        }

        var highestRank =
            (int)(await Database.SortedSetRankAsync(keyProvider.GetKey(), records.First().MemberKeyProvider.GetKey(),
                orderDirection))!;

        if (records.Count == 1)
        {
            return highestRank + 1;
        }

        var sortedRecords = await sortAsync(records);
        var index = sortedRecords.FindIndex(record => record.MemberKeyProvider.GetKey() == id);
        return highestRank + index + 1;
    }

    private static TRecord ToSortedSetRecord(SortedSetEntry sortedSetEntry)
    {
        var record = Activator.CreateInstance<TRecord>();
        record.FromEntry(sortedSetEntry);
        return record;
    }
}
