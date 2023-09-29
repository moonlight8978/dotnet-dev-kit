// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotnetDevelopmentSdk.Lib.Database;

public class BasePostgreSqlDbContext<TDbContext, TDatabaseConfiguration> : DbContext
    where TDbContext : BasePostgreSqlDbContext<TDbContext, TDatabaseConfiguration>
    where TDatabaseConfiguration : DatabaseConfiguration

{
    private readonly DatabaseConfiguration _databaseConfiguration;

    private IDbConnection? _dbConnectionCache;
    public IDbConnection DbConnection => _dbConnectionCache ??= Database.GetDbConnection();

    public BasePostgreSqlDbContext(DbContextOptions<TDbContext> dbContextOptions,
        IOptions<TDatabaseConfiguration> databaseConfiguration)
        : base(dbContextOptions)
    {
        _databaseConfiguration = databaseConfiguration.Value;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_databaseConfiguration.ConnectionString).UseSnakeCaseNamingConvention();
    }
}
