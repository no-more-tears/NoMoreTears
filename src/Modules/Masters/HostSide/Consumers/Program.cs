using Microsoft.Extensions.Hosting;
using NoMoreTears.Modules.Masters.HostSide.Consumers.Registration;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddConsumerHost(configuration);

var app = builder.Build();
await app.RunAsync();
