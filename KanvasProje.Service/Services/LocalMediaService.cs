using KanvasProje.Core.DTOs;
using KanvasProje.Core.Helpers;
using KanvasProje.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KanvasProje.Service.Services
{
    public class LocalMediaService : IMediaService
    {
        private readonly IWebHostEnvironment _env;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly HashSet<string> AllowedVideoExtensions = new(StringComparer.OrdinalIgnoreCase) { ".mp4", ".webm", ".mov" };
        private const int MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB limit

        public LocalMediaService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<FileSaveResultDto> SaveImageAsync(byte[] imageBytes, string fileName, string title, string suffix, int maxWidth = 2000, int maxHeight = 2000)
        {
            try
            {
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    return new FileSaveResultDto { Success = false, ErrorMessage = "Bos dosya." };
                }

                if (imageBytes.Length > MaxImageSizeBytes)
                {
                    return new FileSaveResultDto { Success = false, ErrorMessage = $"Dosya boyutu en fazla 5MB olabilir." };
                }

                var extension = Path.GetExtension(fileName);
                if (!AllowedImageExtensions.Contains(extension))
                {
                    return new FileSaveResultDto { Success = false, ErrorMessage = "Desteklenmeyen gorsel formati." };
                }

                var folder = Path.Combine(_env.WebRootPath, "img", "products");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var slug = SlugHelper.GenerateSlug(title);
                var token = Guid.NewGuid().ToString("N")[..8];
                var finalFileName = $"{slug}-{suffix}-{token}.webp";
                var fullPath = Path.Combine(folder, finalFileName);

                using var stream = new MemoryStream(imageBytes);
                using var image = await Image.LoadAsync(stream);
                
                image.Mutate(x =>
                {
                    x.AutoOrient();
                    x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(maxWidth, maxHeight)
                    });
                });

                await image.SaveAsWebpAsync(fullPath, new WebpEncoder
                {
                    Quality = 82,
                    FileFormat = WebpFileFormatType.Lossy
                });

                return new FileSaveResultDto { Success = true, Url = "/img/products/" + finalFileName };
            }
            catch (Exception ex)
            {
                return new FileSaveResultDto { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<FileSaveResultDto> SaveVideoAsync(byte[] videoBytes, string fileName, string title, string suffix)
        {
            try
            {
                var extension = Path.GetExtension(fileName);
                if (!AllowedVideoExtensions.Contains(extension))
                {
                    return new FileSaveResultDto { Success = false, ErrorMessage = "Desteklenmeyen video formati." };
                }

                var folder = Path.Combine(_env.WebRootPath, "media", "products", "videos");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var slug = SlugHelper.GenerateSlug(title);
                var token = Guid.NewGuid().ToString("N")[..8];
                var finalFileName = $"{slug}-{suffix}-{token}{extension.ToLowerInvariant()}";
                var fullPath = Path.Combine(folder, finalFileName);

                await File.WriteAllBytesAsync(fullPath, videoBytes);

                return new FileSaveResultDto { Success = true, Url = "/media/products/videos/" + finalFileName };
            }
            catch (Exception ex)
            {
                return new FileSaveResultDto { Success = false, ErrorMessage = ex.Message };
            }
        }

        public bool DeleteFile(string relativeUrl)
        {
            if (string.IsNullOrWhiteSpace(relativeUrl)) return false;

            var path = relativeUrl.TrimStart('/');
            var fullPath = Path.Combine(_env.WebRootPath, path);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }
    }
}
