// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Redis;

public abstract class CompositeKeyProvider
{
    private string _fixedKey;

    public virtual string GetKey()
    {
        return _fixedKey ?? ToKey();
    }

    protected abstract string ToKey();

    public virtual void SetKey(string key)
    {
        _fixedKey = key;
    }
}
