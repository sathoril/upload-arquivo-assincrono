using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.IO;
using System.Threading.Tasks;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services.Interfaces;

namespace UpdateArquivoAssincrono.SchedulerJob
{
    [DisallowConcurrentExecution]
    public class ProcessamentoExcelJob : IJob
    {
        private IProcessamentoExcelService processamentoExcel;

        // TODO: Realizar DI do Service
        public ProcessamentoExcelJob(IProcessamentoExcelService processamentoExcel)
        {
            this.processamentoExcel = processamentoExcel;
        }

        public Task Execute(IJobExecutionContext context)
        {
            string diretorio = $"{Directory.GetCurrentDirectory()}\\novosArquivos";
            string nomeNovoArquivo = $"arquivo{DateTime.Now.Date.ToString("dd-MM-yyyy-HH-mm-ss")}";

            this.processamentoExcel.CriarNovoArquivoExcel(diretorio, nomeNovoArquivo);
            return Task.CompletedTask;
        }
    }
}
