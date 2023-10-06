// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DummyApp.Database;

public class AppDbContext : BasePostgreSqlDbContext<AppDbContext, DatabaseConfiguration>
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions,
        IOptions<DatabaseConfiguration> databaseConfiguration) : base(dbContextOptions, databaseConfiguration)
    {
    }
}
