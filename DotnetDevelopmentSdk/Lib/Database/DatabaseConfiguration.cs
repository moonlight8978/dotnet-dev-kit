// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Database;

[CustomConfiguration("Database")]
public class DatabaseConfiguration : ICustomConfiguration
{
    public string Host { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public int Port { get; set; } = 5432;
    public string Database { get; set; }

    public string ConnectionString =>
        $"Server={Host};port={Port};user id={User};password={Password};database={Database};pooling=true";
}
