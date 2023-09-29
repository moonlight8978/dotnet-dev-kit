// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Services.HttpClient;

public abstract class HttpClientConfiguration : ICustomConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutInMillisecond { get; set; } = 10000;
}
