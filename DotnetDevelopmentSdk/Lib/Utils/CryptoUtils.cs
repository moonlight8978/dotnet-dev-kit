// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Security.Cryptography;

namespace DotnetDevelopmentSdk.Lib.Utils;

public class CryptoUtils
{
    private const string Base36Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string SecureRandomBase36(int length)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return new string(bytes.Select(x => Base36Chars[x % Base36Chars.Length]).ToArray());
    }
}
