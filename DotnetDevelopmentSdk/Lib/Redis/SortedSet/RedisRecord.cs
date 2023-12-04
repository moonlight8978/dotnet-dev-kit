// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Redis.SortedSet;

public class RedisRecord<TKey, TData> where TKey : IRedisKey
{
    public TKey Key { get; set; }

    public TData? Data { get; set; }

    public bool HasValue => Data != null;

    public RedisRecord(TKey key, TData? data)
    {
        Key = key;
        Data = data;
    }
}
