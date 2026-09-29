using KanvasProje.Core.DTOs;
using System.Threading.Tasks;

namespace KanvasProje.Service.Interfaces
{
    public interface IMediaService
    {
        Task<FileSaveResultDto> SaveImageAsync(byte[] imageBytes, string fileName, string title, string suffix, int maxWidth = 2000, int maxHeight = 2000);
        Task<FileSaveResultDto> SaveVideoAsync(byte[] videoBytes, string fileName, string title, string suffix);
        bool DeleteFile(string relativeUrl);
    }
}
