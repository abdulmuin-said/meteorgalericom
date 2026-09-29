using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KanvasProje.Core.Helpers
{
    public static class ProductMediaEmbedHelper
    {
        private static readonly HashSet<string> DirectVideoExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4",
            ".webm",
            ".mov",
            ".m4v",
            ".ogg"
        };

        public static bool IsDirectVideoFile(string? source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            var cleanedSource = source.Split('?', '#')[0];
            var extension = Path.GetExtension(cleanedSource);
            return !string.IsNullOrWhiteSpace(extension) && DirectVideoExtensions.Contains(extension);
        }

        public static string? GetEmbedUrl(string? source)
        {
            if (string.IsNullOrWhiteSpace(source) || !Uri.TryCreate(source, UriKind.Absolute, out var uri))
            {
                return null;
            }

            var host = uri.Host.ToLowerInvariant();
            if (host.Contains("youtu.be"))
            {
                var videoId = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                return BuildYouTubeEmbedUrl(videoId);
            }

            if (host.Contains("youtube.com"))
            {
                if (TryReadQueryValue(uri.Query, "v", out var videoId))
                {
                    return BuildYouTubeEmbedUrl(videoId);
                }

                var segments = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && (segments[0].Equals("embed", StringComparison.OrdinalIgnoreCase) || segments[0].Equals("shorts", StringComparison.OrdinalIgnoreCase)))
                {
                    return BuildYouTubeEmbedUrl(segments[1]);
                }
            }

            if (host.Contains("vimeo.com"))
            {
                var videoId = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                return string.IsNullOrWhiteSpace(videoId) ? null : $"https://player.vimeo.com/video/{videoId}";
            }

            return null;
        }

        public static string GetVideoMimeType(string? source)
        {
            var cleanedSource = (source ?? string.Empty).Split('?', '#')[0];
            return Path.GetExtension(cleanedSource).ToLowerInvariant() switch
            {
                ".webm" => "video/webm",
                ".ogg" => "video/ogg",
                ".mov" => "video/quicktime",
                ".m4v" => "video/x-m4v",
                _ => "video/mp4"
            };
        }

        private static string? BuildYouTubeEmbedUrl(string? videoId)
        {
            return string.IsNullOrWhiteSpace(videoId)
                ? null
                : $"https://www.youtube-nocookie.com/embed/{videoId}";
        }

        private static bool TryReadQueryValue(string query, string key, out string? value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                return false;
            }

            foreach (var part in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var pieces = part.Split('=', 2);
                if (pieces.Length == 0)
                {
                    continue;
                }

                if (!pieces[0].Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                value = pieces.Length > 1 ? Uri.UnescapeDataString(pieces[1]) : string.Empty;
                return true;
            }

            return false;
        }
    }
}
