﻿using Microsoft.AspNetCore.Mvc;
using PerfomanceLogger.Api.ServiceInterfaces;

namespace PerfomanceLogger.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfomanceLoggerController : ControllerBase
    {
        private readonly ILogger<PerfomanceLoggerController> _logger;

        public PerfomanceLoggerController(ILogger<PerfomanceLoggerController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadData(IFormFile file, [FromServices]IDocumentService documentService)
        {
            try
            {
                Console.WriteLine(file.Length);
                documentService.UploadCsv(file);
                return Ok();  
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetFilteredData()
        {
            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetFileValues()
        {
            return Ok();
        }
    }
}
