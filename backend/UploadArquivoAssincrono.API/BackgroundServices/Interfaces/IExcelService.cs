using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UploadArquivoAssincrono.API.BackgroundServices.Interfaces
{
    public interface IExcelService
    {
        Task<string> DividirExcel(string caminho, string nomeArquivo);
    }
}
