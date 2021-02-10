using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services;

namespace UpdateArquivoAssincrono.SchedulerJob
{
    [DisallowConcurrentExecution]
    public class ProcessamentoExcelJob : IJob
    {
        private readonly ILogger<ProcessamentoExcelJob> logger;

        public ProcessamentoExcelJob(ILogger<ProcessamentoExcelJob> logger)
        {
            this.logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            ProcessamentoExcelService.ProcessarArquivoExcel();
            return Task.CompletedTask;
        }
    }
}
