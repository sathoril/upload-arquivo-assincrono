using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace UpdateArquivoAssincrono.SchedulerJob
{
    public class Job
    {
        public Job(Type tipoDoJob, string cronExpression)
        {
            this.TipoDoJob = tipoDoJob;
            this.CronExpression = cronExpression;
        }

        public Type TipoDoJob { get; set; }
        public string CronExpression { get; set; }
    }
}
