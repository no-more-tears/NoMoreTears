using MediatR;
using System.Diagnostics;

namespace NoMoreTears.Shared.CQS.Abstractions.Handlers;

public interface IDiagnosticHandler<in TRequest>
    where TRequest : IBaseRequest
{
    void Before(
        TRequest request, 
        Activity? activity);

    void After(
        Activity? activity, 
        TRequest request);
}

public interface IDiagnosticHandler<in TRequest, TResponse>
    where TRequest : IBaseRequest
{
    void Before(
        TRequest request,
        Activity? activity);

    void After(
        Activity? activity,
        TRequest request,
        TResponse response);
}