using System.Text;
using System.Text.Json;

namespace KanvasProje.Service.Services
{
    public interface IAdminSecurityAuditService
    {
        Task LogAsync(HttpContext httpContext, string eventType, string message, string? target = null, string? userId = null, string? userName = null);
    }

    public class AdminSecurityAuditService : IAdminSecurityAuditService
    {
        private static readonly SemaphoreSlim FileLock = new(1, 1);
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = false
        };

        private string LogPath => Path.Combine(_environment.ContentRootPath, "App_Data", "admin-security-audit.log");

        public AdminSecurityAuditService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task LogAsync(HttpContext httpContext, string eventType, string message, string? target = null, string? userId = null, string? userName = null)
        {
            var entry = new AdminSecurityAuditEntry
            {
                EventType = eventType,
                Message = message,
                Target = target ?? string.Empty,
                UserId = userId ?? httpContext.User.FindFirst("sub")?.Value ?? httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? string.Empty,
                UserName = userName ?? httpContext.User.Identity?.Name ?? string.Empty,
                Path = httpContext.Request.Path.Value ?? string.Empty,
                Method = httpContext.Request.Method,
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                CreatedUtc = DateTime.UtcNow
            };

            var directory = Path.GetDirectoryName(LogPath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var line = JsonSerializer.Serialize(entry, _serializerOptions) + Environment.NewLine;

            await FileLock.WaitAsync();
            try
            {
                await File.AppendAllTextAsync(LogPath, line, Encoding.UTF8);
            }
            finally
            {
                FileLock.Release();
            }
        }

        private sealed class AdminSecurityAuditEntry
        {
            public string EventType { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Target { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string Method { get; set; } = string.Empty;
            public string IpAddress { get; set; } = string.Empty;
            public DateTime CreatedUtc { get; set; }
        }
    }
}
