using System.Text.Json;
using KanvasProje.Core.Varliklar;

namespace KanvasProje.Service.Services
{
    public interface IAdminSessionStateService
    {
        Task<AdminSessionState> RegisterSessionAsync(AppUser user, string roleLabel, string? ipAddress);
        Task<AdminSessionState?> GetStateAsync(string userId);
        Task<Dictionary<string, AdminSessionState>> GetStatesAsync(IEnumerable<string> userIds);
        Task<bool> ValidateSessionAsync(string userId, string? sessionToken);
        Task ClearSessionAsync(string userId);
    }

    public class AdminSessionStateService : IAdminSessionStateService
    {
        private static readonly SemaphoreSlim FileLock = new(1, 1);
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true
        };

        private string StatePath => Path.Combine(_environment.ContentRootPath, "App_Data", "admin-session-state.json");

        public AdminSessionStateService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<AdminSessionState> RegisterSessionAsync(AppUser user, string roleLabel, string? ipAddress)
        {
            await FileLock.WaitAsync();
            try
            {
                var states = await ReadStatesInternalAsync();
                states.TryGetValue(user.Id, out var currentState);

                var nextState = currentState ?? new AdminSessionState
                {
                    UserId = user.Id
                };

                nextState.PreviousLoginUtc = nextState.CurrentLoginUtc ?? nextState.PreviousLoginUtc;
                nextState.CurrentLoginUtc = DateTime.UtcNow;
                nextState.CurrentSessionToken = Guid.NewGuid().ToString("N");
                nextState.LastIpAddress = ipAddress ?? string.Empty;
                nextState.LastRoleLabel = roleLabel;

                states[user.Id] = nextState;
                await SaveStatesInternalAsync(states);

                return nextState;
            }
            finally
            {
                FileLock.Release();
            }
        }

        public async Task<AdminSessionState?> GetStateAsync(string userId)
        {
            await FileLock.WaitAsync();
            try
            {
                var states = await ReadStatesInternalAsync();
                return states.TryGetValue(userId, out var state) ? state : null;
            }
            finally
            {
                FileLock.Release();
            }
        }

        public async Task<Dictionary<string, AdminSessionState>> GetStatesAsync(IEnumerable<string> userIds)
        {
            var idSet = userIds.ToHashSet(StringComparer.Ordinal);

            await FileLock.WaitAsync();
            try
            {
                var states = await ReadStatesInternalAsync();
                return states
                    .Where(x => idSet.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
            }
            finally
            {
                FileLock.Release();
            }
        }

        public async Task<bool> ValidateSessionAsync(string userId, string? sessionToken)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(sessionToken))
            {
                return false;
            }

            await FileLock.WaitAsync();
            try
            {
                var states = await ReadStatesInternalAsync();
                return states.TryGetValue(userId, out var state) &&
                    string.Equals(state.CurrentSessionToken, sessionToken, StringComparison.Ordinal);
            }
            finally
            {
                FileLock.Release();
            }
        }

        public async Task ClearSessionAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            await FileLock.WaitAsync();
            try
            {
                var states = await ReadStatesInternalAsync();
                if (states.Remove(userId))
                {
                    await SaveStatesInternalAsync(states);
                }
            }
            finally
            {
                FileLock.Release();
            }
        }

        private async Task<Dictionary<string, AdminSessionState>> ReadStatesInternalAsync()
        {
            if (!File.Exists(StatePath))
            {
                return new Dictionary<string, AdminSessionState>(StringComparer.Ordinal);
            }

            try
            {
                var json = await File.ReadAllTextAsync(StatePath);
                return JsonSerializer.Deserialize<Dictionary<string, AdminSessionState>>(json) ??
                    new Dictionary<string, AdminSessionState>(StringComparer.Ordinal);
            }
            catch
            {
                return new Dictionary<string, AdminSessionState>(StringComparer.Ordinal);
            }
        }

        private async Task SaveStatesInternalAsync(Dictionary<string, AdminSessionState> states)
        {
            var directory = Path.GetDirectoryName(StatePath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(states, _serializerOptions);
            await File.WriteAllTextAsync(StatePath, json);
        }
    }

    public class AdminSessionState
    {
        public string UserId { get; set; } = string.Empty;
        public string CurrentSessionToken { get; set; } = string.Empty;
        public DateTime? CurrentLoginUtc { get; set; }
        public DateTime? PreviousLoginUtc { get; set; }
        public string LastIpAddress { get; set; } = string.Empty;
        public string LastRoleLabel { get; set; } = string.Empty;
    }
}
