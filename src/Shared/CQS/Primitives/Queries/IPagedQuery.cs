namespace NoMoreTears.Shared.CQS.Primitives.Queries;

public interface IPagedQuery
{
    int PageNumber { get; }

    int PageSize { get;  }
}