// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Redis.SortedSet;

public interface IRedisKey
{
    public string Prepare();

    public void Consume(string key);

    public TimeSpan? Ttl { get; }
}

public class SimpleRedisKey : IRedisKey
{
    public string Key { get; set; } = Guid.NewGuid().ToString();

    public SimpleRedisKey() { }

    public SimpleRedisKey(string key)
    {
        Key = key;
    }

    public string Prepare()
    {
        return Key;
    }

    public void Consume(string key)
    {
        Key = key;
    }

    public TimeSpan? Ttl => null;
}
