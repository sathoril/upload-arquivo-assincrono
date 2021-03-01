using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services.Interfaces;

namespace UpdateArquivoAssincrono.SchedulerJob.InjectionConfig.Jobs
{
    public class ServicesInjectors
    {
        public static void Config(IServiceCollection services)
        {
            services.AddTransient<IProcessamentoExcelService, ProcessamentoExcelService>();
        }
    }
}
