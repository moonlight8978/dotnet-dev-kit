// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Redis;

public abstract class GenericRedisRecord<T, TKeyProvider> : BaseRedisRecord where TKeyProvider : CompositeKeyProvider
{
    public T? Value { get; set; }

    protected GenericRedisRecord(TKeyProvider memberKeyProvider, T? value = default) : base(memberKeyProvider)
    {
        Value = value;
    }

    public bool HasValue => Value != null;
}
