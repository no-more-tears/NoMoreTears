using Microsoft.Extensions.Hosting;
using NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Registration;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddBackgroundJobsHost(configuration);

var app = builder.Build();
await app.RunAsync();