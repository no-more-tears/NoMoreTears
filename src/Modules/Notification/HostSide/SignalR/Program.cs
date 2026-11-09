using NoMoreTears.Modules.Notification.HostSide.SignalR.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalRHost(builder.Configuration);

var app = builder.Build();
app.UseSignalRHostPipeline();

await app.RunAsync();