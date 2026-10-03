namespace NoMoreTears.Shared.CQS.Primitives.Queries;

public interface ICachedQuery
{
    string  CacheKey { get; }

    TimeSpan? Expiration { get;  }
}