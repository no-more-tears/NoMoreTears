namespace NoMoreTears.Shared.CQS.Primitives.Queries;

public record PagedQuery<TResposne>(
    int PageNumber,
    int PageSize) 
    : IPagedQuery, IQuery<TResposne>;