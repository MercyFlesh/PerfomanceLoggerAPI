namespace PerfomanceLogger.Domain.Interfaces
{
    public interface IDocumentService
    {
        Task UploadCsv(Stream stream, string fileName);
    }
}
