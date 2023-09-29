// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Redis;

[CustomConfiguration("Redis")]
public class RedisConfiguration : StackExchange.Redis.Extensions.Core.Configuration.RedisConfiguration, ICustomConfiguration
{
}
