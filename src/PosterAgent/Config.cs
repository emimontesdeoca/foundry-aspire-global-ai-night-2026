namespace PosterAgent;

/// <summary>Reads environment variables supplied by Aspire, or set manually outside Aspire.</summary>
public static class Config
{
    public static string? Env(string name) =>
        Environment.GetEnvironmentVariable(name) is { Length: > 0 } value ? value : null;

    public static string RequiredEnv(string name) =>
        Env(name) ?? throw new InvalidOperationException(
            $"Required environment variable '{name}' is not set.");

    public static Uri ProjectEndpoint()
    {
        var value = Env("PROJECT_GLOBALAINIGHT_URI")
            ?? Env("FOUNDRY_PROJECT_ENDPOINT")
            ?? throw new InvalidOperationException(
                "Could not find the Foundry project endpoint. Run `aspire run` from src/ " +
                "or set FOUNDRY_PROJECT_ENDPOINT.");

        // Aspire's URI includes ?api-version=...; AIProjectClient only needs the path.
        return new Uri(new Uri(value).GetLeftPart(UriPartial.Path));
    }
}
