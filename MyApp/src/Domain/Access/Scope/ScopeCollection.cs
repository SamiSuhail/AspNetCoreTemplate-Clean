namespace MyApp.Domain.Access.Scope;

public struct ScopeCollection
{
    public const char Separator = ';';
    public HashSet<string> Collection { get; private set; }
    private string _stringRepresentation;

    public static ScopeCollection Empty => new() { Collection = [], _stringRepresentation = string.Empty };

    public static ScopeCollection Create(string scopesAsString)
        => new()
        {
            _stringRepresentation = scopesAsString,
            Collection = [.. scopesAsString.Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)],
        };

    public static ScopeCollection Create(IEnumerable<string> scopes)
        => new()
        {
            _stringRepresentation = string.Join(Separator, scopes),
            Collection = [.. scopes],
        };

    public readonly override string ToString()
        => _stringRepresentation;
}
