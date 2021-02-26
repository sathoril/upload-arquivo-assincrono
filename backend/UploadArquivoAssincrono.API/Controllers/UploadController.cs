using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UploadArquivoAssincrono.API.BackgroundServices.Interfaces;
using UploadArquivoAssincrono.API.ImportacaoExcel;

namespace UploadArquivoAssincrono.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        
        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("excel")]
        public async Task<IActionResult> Excel(IFormFile file)
        {
            bool sucesso;
            try
            {
                sucesso = await this.processarArquivo(file);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(sucesso);
        }

        private async Task<bool> processarArquivo(IFormFile arquivo)
        {
            bool sucesso;
            try
            {
                string fileName = $"arquivoOriginal_{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}.xlsx";
                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), $"Upload\\files\\{DateTime.Now.Year}\\{DateTime.Now.Month}\\{DateTime.Now.Day}\\{DateTime.Now.Hour}\\{DateTime.Now.Minute}");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                var path = Path.Combine(pathBuilt, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                BackgroundJob.Enqueue<ExcelService>(x => x.DividirExcel(pathBuilt, fileName));

                sucesso = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return sucesso;
        }
    }
}
