using MediatR;
using System.Diagnostics;

namespace NoMoreTears.Shared.CQS.Abstractions.Handlers;

public abstract class BaseDiagnosticHandler<TRequest, TResponse>
    : IDiagnosticHandler<TRequest>
    where TRequest : IBaseRequest
{
    public abstract void After(Activity? activity, TRequest request);

    public abstract void Before(TRequest request, Activity? activity);

    protected static void SetTag(Activity? activity, string key, object? value)
        => activity?.SetTag(key, value);

    protected static void SetBaggage(Activity? activity, string key, string? value)
      => activity?.SetBaggage(key, value);

    protected static void AddEvent(Activity? activity, string name, ActivityTagsCollection? tags = null)
       => activity?.AddEvent(new ActivityEvent(name, tags: tags ?? []));
}