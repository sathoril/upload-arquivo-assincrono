using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services.Interfaces;

namespace UpdateArquivoAssincrono.SchedulerJob.InjectionConfig.Services
{
    public class ServicesInjectors
    {
        public static void Config(IServiceCollection services)
        {
            services.AddSingleton<ProcessamentoExcelJob>();

            foreach (Job job in listarJobs())
            {
                services.AddSingleton(job);
            }
        }

        private static List<Job> listarJobs()
        {
            var jobs = new List<Job>();

            jobs.Add(new Job(tipoDoJob: typeof(ProcessamentoExcelJob), cronExpression: "0/30 * * * * ?"));

            return jobs;
        }
    }
}
