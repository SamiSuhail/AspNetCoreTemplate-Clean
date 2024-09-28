namespace MyApp.Domain;

public class DomainException(DomainFailure failure) 
    : Exception($"The following errors have occured: {string.Join(", ", failure.Errors.Select(e => $"({e.Key}: {e.Message})").ToArray())}")
{
    public DomainFailure Failure { get; set; } = failure;
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

