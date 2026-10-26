using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoMoreTears.Modules.Identity.HostSide.Migrations.Registration;
using NoMoreTears.Shared.Postgres.Migrations.Abstractions;
using NoMoreTears.Shared.Providers.Identity.Migrations;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddMigrationHost<AssemblyReference>(configuration);

var app = builder.Build();

var migrator = app.Services.GetRequiredService<IDbMigrationManager>();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger(HostRegistrar.LoggerCategory);

using var cts = new CancellationTokenSource();

await migrator.ApplyMigrationsAsync(logger, cts.Token);

return 0;