using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.ExceptionHandling;

/// <summary>
///     Web module responsible for global exception handling and 
///     standardized error responses using ProblemDetails.
/// </summary>
public sealed class ExceptionHandlingWebModule : IWebModule
{
    /// <inheritdoc />
    public int Order => WebModuleOrder.ExceptionHandling;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        context.Services
           .AddExceptionHandler<GlobalExceptionHandler>()
           .AddProblemDetails(options =>
           {
               // Customize ProblemDetails for more detailed error responses.
               options.CustomizeProblemDetails = context =>
               {
                   context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                   context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                   var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                   context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
               };
           });
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
        app.UseExceptionHandler();
    }
}