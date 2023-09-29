// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Authentication;

public class JwtTokenSignedData
{
    public string Token { get; set; } = null!;
    public DateTime ExpireTime { get; set; }
}
