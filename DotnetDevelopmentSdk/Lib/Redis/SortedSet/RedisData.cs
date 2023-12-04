// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Newtonsoft.Json;

namespace DotnetDevelopmentSdk.Lib.Redis.SortedSet;

public interface IRedisData
{
    public string Prepare();

    public void Consume(string data);
}

public class JsonRedisData<T> : IRedisData
{
    public T? Data { get; set; }

    public string Prepare()
    {
        return Data == null ? string.Empty : JsonConvert.SerializeObject(Data);
    }

    public void Consume(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return;
        }

        Data = JsonConvert.DeserializeObject<T>(data)!;
    }
}
