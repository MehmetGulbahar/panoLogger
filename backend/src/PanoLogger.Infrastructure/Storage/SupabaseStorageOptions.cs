namespace PanoLogger.Infrastructure.Storage;

public sealed class SupabaseStorageOptions
{
    public const string SectionName = "Supabase";

    public string Url { get; init; } = "";
    public string ServiceRoleKey { get; init; } = "";
    public string StorageBucket { get; init; } = "panel-files";
    public string? PostgresConnectionString { get; init; }
    public string PostgresHost { get; init; } = "";
    public int PostgresPort { get; init; } = 5432;
    public string PostgresDatabase { get; init; } = "postgres";
    public string PostgresUsername { get; init; } = "";
    public string PostgresPassword { get; init; } = "";
    public long MaxFileBytes { get; init; } = 10 * 1024 * 1024;
    public int SignedUrlExpirationSeconds { get; init; } = 300;
    public string[] AllowedContentTypes { get; init; } =
    [
        "application/pdf",
        "image/jpeg",
        "image/png",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    ];
    public string[] AllowedExtensions { get; init; } =
    [
        ".pdf",
        ".jpg",
        ".jpeg",
        ".png",
        ".docx"
    ];
}
