using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services.Interfaces;

namespace UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services
{
    public class ProcessamentoExcelService : IProcessamentoExcelService
    {
        public ProcessamentoExcelService()
        {
        }

        public void ProcessarArquivoExcel()
        {
            string diretorioAtual = Directory.GetCurrentDirectory();
            var pathBuilt = Path.Combine(Directory.GetParent(diretorioAtual).FullName, "UploadArquivoAssincrono.API\\Upload\\files");

            if (!Directory.Exists(pathBuilt))
                return;

            string[] arquivosNoDiretorio = Directory.GetFiles(pathBuilt);
            if (arquivosNoDiretorio == null || arquivosNoDiretorio.Length == 0)
                return;

            string caminhoArquivo = arquivosNoDiretorio[0];
            if (File.Exists(caminhoArquivo))
                File.Delete(caminhoArquivo);

            return;
        }
    }
}
