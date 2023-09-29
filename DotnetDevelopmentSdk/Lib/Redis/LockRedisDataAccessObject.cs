// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DotnetDevelopmentSdk.Lib.Redis;

public class LockRedisDataAccessObject
{
    private readonly IRedisDatabase _redisDatabase;

    public LockRedisDataAccessObject(IRedisDatabase redisDatabase)
    {
        _redisDatabase = redisDatabase;
    }

    public async Task<T> LockAsync<T>(CompositeKeyProvider compositeKeyProvider, TimeSpan duration,
        Func<Task<T>> action)
    {
        var locksCount = 0;

        var lockOwner = Guid.NewGuid().ToString();
        try
        {
            while (!await _redisDatabase.Database.LockTakeAsync(compositeKeyProvider.GetKey(), lockOwner, duration))
            {
                if (locksCount >= 20)
                {
                    throw new Exception("Lock failed");
                }

                locksCount += 1;
                await Task.Delay(100);
            }

            return await action.Invoke();
        }
        finally
        {
            await _redisDatabase.Database.LockReleaseAsync(compositeKeyProvider.GetKey(), lockOwner);
        }
    }

    public async Task LockAsync(CompositeKeyProvider compositeKeyProvider, TimeSpan duration, Func<Task> action)
    {
        await LockAsync(compositeKeyProvider, duration, async () =>
        {
            await action.Invoke();
            return new { };
        });
    }
}
