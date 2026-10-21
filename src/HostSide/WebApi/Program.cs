using NoMoreTears.HostSide.WebApi.Registration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddHostModules(builder.Configuration);
services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.RunAsync();