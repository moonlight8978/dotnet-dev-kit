// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Newtonsoft.Json;
using Npgsql;

namespace DotnetDevelopmentSdk.Lib.Database;

public static class PostgresqlInitializer
{
    public static void Initialize()
    {
        NpgsqlConnection.GlobalTypeMapper.UseJsonNet(settings: new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        });
    }
}
