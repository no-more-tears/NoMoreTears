namespace NoMoreTears.Modules.Masters.Contracts.Requests;

public sealed record ChangeFullNameRequest(
    string FirstName,
    string LastName);
