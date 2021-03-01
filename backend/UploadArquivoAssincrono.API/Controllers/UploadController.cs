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
        private readonly string nomeDoArquivo = $"arquivoOriginal_{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}-{DateTime.Now.Hour}-{DateTime.Now.Minute}.xlsx";
        private readonly string caminhoDoArquivo = Path.Combine(Directory.GetCurrentDirectory(), $"Upload\\files\\{DateTime.Now.Year}\\{DateTime.Now.Month}\\{DateTime.Now.Day}\\{DateTime.Now.Hour}\\{DateTime.Now.Minute}");

        public UploadController() { }

        /// <summary>
        /// Realiza o upload do arquivo excel para ser processado e dividido em arquivos menores.
        /// </summary>
        /// <param name="arquivoExcel">Arquivo excel</param>
        /// <returns>200OK - Upload realizado com sucesso!</returns>
        /// <response code="404">Retorna 404NotFound se o arquivo não conseguiu ser salvo para processamento.</response>  
        /// /// <response code="404">Retorna 404NotFound se o arquivo não conseguiu ser salvo para processamento.</response>  
        [HttpPost]
        [Route("excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Excel(IFormFile arquivoExcel)
        {
            try
            {
                if (!Directory.Exists(this.caminhoDoArquivo))
                {
                    Directory.CreateDirectory(this.caminhoDoArquivo);
                }

                string caminho = Path.Combine(this.caminhoDoArquivo, this.nomeDoArquivo);
                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await arquivoExcel.CopyToAsync(stream);
                }

                BackgroundJob.Enqueue<ExcelService>(x => x.IniciarDivisao(caminhoDoArquivo, nomeDoArquivo));
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok("Upload realizado com sucesso!");
        }
    }
}
