// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Utils;

public class FakerUtils
{
    private readonly Dictionary<string, HashSet<int>> _labelToValuePool = new();

    public static string RandomizeEthAddress()
    {
        var random = new Random();
        var bytes = new byte[20];
        random.NextBytes(bytes);
        return string.Concat("0x", string.Concat(Array.ConvertAll(bytes, x => x.ToString("X2"))).ToLower());
    }

    public T Unique<T>(string label, Func<T> valueGenerator)
    {
        if (_labelToValuePool.TryGetValue(label, out var pool))
        {
            while (true)
            {
                var value = valueGenerator.Invoke();
                var valueHash = value.GetHashCode();
                if (pool.Contains(value.GetHashCode()))
                {
                    continue;
                }

                pool.Add(valueHash);
                return value;
            }
        }
        else
        {
            var value = valueGenerator.Invoke();
            _labelToValuePool.Add(label, new HashSet<int> { value.GetHashCode() });
            return value;
        }
    }
}
