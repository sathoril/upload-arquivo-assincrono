using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateArquivoAssincrono.SchedulerJob
{
    public class HostedService : IHostedService
    {
        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobFactory jobFactory;
        private readonly IEnumerable<Job> jobs;

        public IScheduler Scheduler { get; set; }

        public HostedService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IEnumerable<Job> jobs)
        {
            this.schedulerFactory = schedulerFactory;
            this.jobFactory = jobFactory;
            this.jobs = jobs;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await this.schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = this.jobFactory;

            foreach (var jobSchedule in this.jobs)
            {
                var job = CriarJob(jobSchedule);
                var trigger = CriarGatilhoDeExecucao(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }

        private static IJobDetail CriarJob(Job job)
        {
            var jobType = job.TipoDoJob;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.Name)
                .Build();
        }

        private static ITrigger CriarGatilhoDeExecucao(Job job)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{job.TipoDoJob.FullName}.trigger")
                .WithCronSchedule(job.CronExpression)
                .WithDescription(job.CronExpression)
                .Build();
        }
    }
}
