using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using UpdateArquivoAssincrono.SchedulerJob.InjectionConfig.Jobs;
using UpdateArquivoAssincrono.SchedulerJob.InjectionConfig.Services;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services.Interfaces;

namespace UpdateArquivoAssincrono.SchedulerJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    ServicesInjectors.Config(services);

                    // Adicionamos nossa implementa��o do IHostedService, dizemos que esta aplica��o rodar� um Worker Service
                    services.AddHostedService<HostedService>();

                    /* 
                     * Adicionamos o Singleton de duas implementa��es, a primeira uma implementa��o simples da interface IJobFactory, 
                     * apenas para iniciarmos os jobs e na segunda uma implementa��o do pr�prio Quartz.
                     * 
                     * Criamos uma implementa��o da JobFactory para podermos trabalhar com o container de DI do .NETe assim, basicamente, como ela segue o 
                     * pattern Factory, ela consegue implementar v�rias interfaces diferentes, sendo essas interfaces cada job criado.
                    */
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

                    JobsInjectors.Config(services);
                    
                });
    }
}
