// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Utils;

public static class NumberExtensions
{
    public static bool IsEven(this int value)
    {
        return value % 2 == 0;
    }
}
