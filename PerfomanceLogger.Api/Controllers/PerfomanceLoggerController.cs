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
        public async Task<IActionResult> UploadData(IFormFile file, [FromServices]IDocumentService documentService)
        {
            if (file.Length == 0)
                return BadRequest("Received empty file");

            string format = file.FileName.Split('.')[1];
            if (format != "csv")
                return BadRequest("Incorrect file format");

            try
            {
                await documentService.UploadCsv(file.OpenReadStream(), file.FileName.Split('.')[0]);
                return Ok();  
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetFilteredData([FromQuery]FilterQuery filter)
        {
            Console.WriteLine(filter.FileName == null);
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetFileValues(string fileName)
        {
            return Ok();
        }
    }
}
