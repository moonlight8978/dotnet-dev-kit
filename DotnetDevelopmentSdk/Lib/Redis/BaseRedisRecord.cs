// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Redis;

public abstract class BaseRedisRecord
{
    public CompositeKeyProvider MemberKeyProvider { get; protected set; }

    protected BaseRedisRecord(CompositeKeyProvider memberKeyProvider)
    {
        MemberKeyProvider = memberKeyProvider;
    }
}
