// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Serilog;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace DotnetDevelopmentSdk.Lib.Redis;

public abstract class BaseRedisDataAccessObject
{
    protected readonly IDatabase Database;

    protected readonly ILogger Logger;

    protected abstract TimeSpan ExpirationTime { get; }

    protected BaseRedisDataAccessObject(IRedisDatabase redisDatabase)
    {
        Database = redisDatabase.Database;
        Logger = Log.ForContext(GetType());
    }

    protected async Task UseTransaction(ITransaction? transaction, List<Func<IDatabaseAsync, Task>> makeTasks,
        int batchSize = 5, bool submitTx = true)
    {
        if (transaction == null)
        {
            foreach (var makeTasksSubset in makeTasks.Chunk(batchSize))
            {
                await Task.WhenAll(makeTasksSubset.Select(makeTask => makeTask(Database)));
            }
        }
        else
        {
            foreach (var makeTask in makeTasks)
            {
#pragma warning disable 4014
                makeTask(transaction);
#pragma warning restore 4014
            }

            if (submitTx)
            {
                await transaction.ExecuteAsync();
            }
        }
    }
}
