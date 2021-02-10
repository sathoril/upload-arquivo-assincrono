using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services
{
    public static class ProcessamentoExcelService
    {
        public static void ProcessarArquivoExcel()
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
