using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services.Interfaces
{
    public interface IProcessamentoExcelService
    {
        // Task ProcessarArquivoExcelAsync();
        void CriarNovoArquivoExcel(string caminho, string nomeDoArquivo);
    }
}
