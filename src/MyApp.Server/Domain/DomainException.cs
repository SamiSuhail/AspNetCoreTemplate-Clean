namespace MyApp.Server.Domain;

public class DomainException : Exception
{
    public DomainException(DomainFailure failure) : base($"The following errors have occured: {string.Join(", ", failure.Errors.Select(e => $"({e.Key}: {e.Message})").ToArray())}")
    {
        Failure = failure;
    }
    public DomainFailure Failure { get; set; }
}

public class DomainFailure
{
    public DomainFailure AddError(string key, string message)
    {
        Errors.Add(new(key, message));
        return this;
    }

    public DomainFailure AddErrors(IEnumerable<DomainError> errors)
    {
        Errors.AddRange(errors);
        return this;
    }

    public DomainException ToException()
        => new(this);
    public void ThrowOnError()
    {
        if (Errors.Count > 0)
            throw new DomainException(this);
    }

    public List<DomainError> Errors { get; set; } = [];
}

public class DomainError(string key, string message)
{
    public string Key { get; set; } = key;
    public string Message { get; set; } = message;
}

