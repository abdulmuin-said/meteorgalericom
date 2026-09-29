namespace KanvasProje.Core.DTOs
{
    public class FileSaveResultDto
    {
        public bool Success { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
