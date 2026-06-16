using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PanoLogger.Application.Common.Interfaces;
using PanoLogger.Infrastructure.Auditing;
using PanoLogger.Infrastructure.Authentication;
using PanoLogger.Infrastructure.Persistence;
using PanoLogger.Infrastructure.Qr;
using PanoLogger.Infrastructure.Storage;

namespace PanoLogger.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var supabaseOptions = configuration.GetSection(SupabaseStorageOptions.SectionName).Get<SupabaseStorageOptions>();
        var connectionString = !string.IsNullOrWhiteSpace(supabaseOptions?.PostgresConnectionString)
            ? supabaseOptions.PostgresConnectionString
            : CreateSupabaseConnectionString(supabaseOptions)
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=pano_logger;Username=postgres;Password=postgres";

        services.AddDbContext<PanoLoggerDbContext>(options => options.UseNpgsql(connectionString));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<QrCodeOptions>(configuration.GetSection(QrCodeOptions.SectionName));
        services.Configure<SupabaseStorageOptions>(configuration.GetSection(SupabaseStorageOptions.SectionName));
        services.AddScoped<IAuditLogWriter, DbAuditLogWriter>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
        services.AddSingleton<IQrCodeService, QrCodeService>();
        services.AddSingleton<IPanelCodeService, PanelCodeService>();
        services.AddSingleton<IFileSecurityService, FileSecurityService>();
        services.AddHttpClient<IFileStorageService, SupabaseStorageService>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<SupabaseStorageOptions>>()
                .Value;

            if (string.IsNullOrWhiteSpace(options.Url) || string.IsNullOrWhiteSpace(options.ServiceRoleKey))
            {
                return;
            }

            httpClient.BaseAddress = new Uri(options.Url.TrimEnd('/'));
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.ServiceRoleKey);
            httpClient.DefaultRequestHeaders.Add("apikey", options.ServiceRoleKey);
        });

        return services;
    }

    private static string? CreateSupabaseConnectionString(SupabaseStorageOptions? options)
    {
        if (options is null || string.IsNullOrWhiteSpace(options.PostgresHost))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(options.PostgresUsername))
        {
            throw new InvalidOperationException("Supabase:PostgresUsername is required when Supabase:PostgresHost is configured.");
        }

        if (string.IsNullOrWhiteSpace(options.PostgresPassword))
        {
            throw new InvalidOperationException(
                "Supabase:PostgresPassword is required when Supabase:PostgresHost is configured. "
                + "Store it with dotnet user-secrets instead of appsettings.");
        }

        return new NpgsqlConnectionStringBuilder
        {
            Host = options.PostgresHost,
            Port = options.PostgresPort,
            Database = options.PostgresDatabase,
            Username = options.PostgresUsername,
            Password = options.PostgresPassword,
            SslMode = SslMode.Require,
            GssEncryptionMode = GssEncryptionMode.Disable,
        }.ConnectionString;
    }
}
