namespace NoMoreTears.Shared.CQS.Abstractions.Handlers;

public interface ICorrelationIdAccessor
{
    string CorrelationId { get; }
}