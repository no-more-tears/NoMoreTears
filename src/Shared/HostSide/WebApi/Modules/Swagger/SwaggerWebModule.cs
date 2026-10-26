using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Swagger;

public sealed class SwaggerWebModule : IWebModule
{
    private const string BearerSchemeName = "Bearer";

    public int Order => WebModuleOrder.Swagger;

    public void MountServices(WebModuleContext context)
    {
        context.Services
            .AddOpenApi()
            .AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NoMoreTears API",
                    Version = "v1",
                    Description = "API documentation for NoMoreTears",
                    Contact = new OpenApiContact
                    {
                        Name = "NoMoreTears Support",
                        Email = "support@noreply.com"
                    }
                });

                options.AddSecurityDefinition(BearerSchemeName, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token without the 'Bearer ' prefix."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(BearerSchemeName, document)] = []
                });
            });
    }

    public void MountPipeline(IApplicationBuilder app)
    {
        var environment = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DisplayOperationId();
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.EnableDeepLinking();
                options.EnableTryItOutByDefault();
                options.EnablePersistAuthorization();
                options.DocExpansion(DocExpansion.None);
            });
        }
    }
}