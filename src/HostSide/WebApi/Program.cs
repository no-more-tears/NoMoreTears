using NoMoreTears.HostSide.WebApi.Registration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;
var host = builder.Host;

services
    .AddWebApiHost(configuration, environment, host)
    .AddWebApiHostModules(configuration);

var app = builder.Build();
app.UseWebApiHostPipeline();

await app.RunAsync();