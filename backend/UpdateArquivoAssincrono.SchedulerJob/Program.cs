using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

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

                    services.AddSingleton<ProcessamentoExcelJob>();
                    services.AddSingleton(new Job(
                        tipoDoJob: typeof(ProcessamentoExcelJob),
                        cronExpression: "0/30 * * * * ?"));
                });
    }
}
