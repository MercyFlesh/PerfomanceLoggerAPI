using Microsoft.AspNetCore.Mvc;
using PerfomanceLogger.Domain.Models;
using PerfomanceLogger.Domain.Interfaces;

namespace PerfomanceLogger.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfomanceLoggerController : ControllerBase
    {
        private readonly IPerfomanceRepository _perfomanceRepository;
        private readonly ILogger<PerfomanceLoggerController> _logger;

        public PerfomanceLoggerController(
            IPerfomanceRepository perfomanceRepository,
            ILogger<PerfomanceLoggerController> logger)
        {
            _perfomanceRepository = perfomanceRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadData(IFormFile file, [FromServices] IDocumentService documentService)
        {
            if (file.Length == 0)
                throw new ArgumentException("Received empty file");

            string format = file.FileName.Split('.')[1];
            if (format != "csv")
                throw new ArgumentException("Incorrect file format");

           await documentService.UploadCsv(file.OpenReadStream(), file.FileName.Split('.')[0]);
           return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetResults([FromQuery] FilterQuery filter)
        {
            return new JsonResult(await _perfomanceRepository.GetResultsAsync(filter));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetFileValues([FromQuery] string fileName)
        {
            return new JsonResult(await _perfomanceRepository.GetValuesByFileNameAsync(fileName));
        }
    }
}
