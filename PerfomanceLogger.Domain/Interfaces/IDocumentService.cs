namespace PerfomanceLogger.Domain.Interfaces
{
    public interface IDocumentService
    {
        void UploadCsv(Stream stream, string fileName);
    }
}
