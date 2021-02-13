using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services.Interfaces;

namespace UpdateArquivoAssincrono.SchedulerJob.Jobs.ProcessamentoExcel.Services
{
    public class ProcessamentoExcelService : IProcessamentoExcelService
    {
        public void CriarNovoArquivoExcel(string caminho, string nomeDoArquivo)
        {
            // Procurar arquivo excel
            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            File.Create($"{caminho}\\{nomeDoArquivo}.xlsx");
        }
    }
}

