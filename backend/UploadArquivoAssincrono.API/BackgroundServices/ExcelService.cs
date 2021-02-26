using ClosedXML.Excel;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UploadArquivoAssincrono.API.BackgroundServices.Interfaces;

namespace UploadArquivoAssincrono.API.ImportacaoExcel
{
    public class ExcelService
    {
        private readonly int LINHAS_MAXIMA_POR_ARQUIVO = 1000;
        private readonly string NOVA_PASTA = "excels-para-processamento";

        public void DividirExcel(string caminho, string nomeArquivo)
        {

            Console.WriteLine($"Iniciando processamento/quebra do arquivo excel informado!");
            Console.WriteLine($"---------------------------------------------------------- \n");

            Console.WriteLine($"Iniciando quebra do arquivo {nomeArquivo}");
            Console.WriteLine($"---------------------------------------------------------- \n");

            try
            {
                string novoDiretorio = IniciarDivisao(caminho, nomeArquivo);

                BackgroundJob.Enqueue<ExcelService>(
                    x => x.ImportarExcel(novoDiretorio));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao realizar o processamento do excel. \n");
                Console.WriteLine($"{ex.InnerException}\n");
            }
        }

        private void ImportarExcel(string novoDiretorio)
        {
            // TODO: Executar importação do excel para tabelas no BD
        }

        private string IniciarDivisao(string caminhoExcel, string nomeArquivo)
        {
            string novoDiretorio = $"{caminhoExcel}\\{NOVA_PASTA}";

            // Procurar arquivo excel
            VerificarSeArquivoExiste(caminhoExcel, nomeArquivo);

            // Transformar arquivo excel em Workbook
            using (XLWorkbook arquivoExcel = new XLWorkbook($"{caminhoExcel}\\{nomeArquivo}"))
            {
                int numeroDeWorksheets = arquivoExcel.Worksheets.Count;
                if (numeroDeWorksheets <= 0)
                    throw new FileNotFoundException();

                var worksheetOriginal = arquivoExcel.Worksheets.FirstOrDefault();
                int totalDeLinhasNoArquivo = worksheetOriginal.RowsUsed().Count();
                Console.WriteLine($"Arquivo Excel possui {totalDeLinhasNoArquivo} no total!\n");

                int numeroDeArquivosNoTotal = (totalDeLinhasNoArquivo - 1) / LINHAS_MAXIMA_POR_ARQUIVO;
                if (numeroDeArquivosNoTotal <= 0)
                    numeroDeArquivosNoTotal = 1;

                Console.WriteLine($"Serão criados {numeroDeArquivosNoTotal} arquivos excel no total! \n");

                for (int numeroDoNovoArquivo = 0; numeroDoNovoArquivo < numeroDeArquivosNoTotal; numeroDoNovoArquivo++)
                {
                    XLWorkbook novoArquivoExcel = new XLWorkbook();
                    DataTable dataTable = new DataTable($"ExcelGerado-{numeroDoNovoArquivo + 1}");

                    // Cria cabeçalho do novo arquivo
                    MontarCabecalhoDoNovoArquivo(dataTable, worksheetOriginal);

                    Console.WriteLine($"Montado cabeçalho do arquivo número {numeroDoNovoArquivo + 1} \n");

                    ProcessarLinhas(worksheetOriginal, dataTable);

                    SalvarNovoArquivo(caminhoExcel, numeroDoNovoArquivo, novoArquivoExcel, dataTable, novoDiretorio);
                }
            }

            return novoDiretorio;
        }

        private void ProcessarLinhas(IXLWorksheet worksheetOriginal, DataTable dataTable)
        {
            int linhasProcessadas = 0;
            bool linhaDoCabecalho = true;
            foreach (var linha in worksheetOriginal.RowsUsed())
            {
                // Ignora processamento para a linha do cabeçalho
                if (linhaDoCabecalho)
                {
                    linhaDoCabecalho = false;
                    continue;
                }

                if (linhasProcessadas == LINHAS_MAXIMA_POR_ARQUIVO)
                {
                    Console.WriteLine($"Já foram processadas {linhasProcessadas} linhas, finalizando criação de novo excel! \n");
                    break;
                }

                dataTable.Rows.Add();

                ProcessarColunas(dataTable, linha);

                linhasProcessadas++;
            }
        }

        private static void ProcessarColunas(DataTable dataTable, IXLRow linha)
        {
            foreach (var item in linha.CellsUsed())
            {
                dataTable.Rows[dataTable.Rows.Count - 1][item.Address.ColumnNumber - 1] = item.Value.ToString();
            }
        }

        private string SalvarNovoArquivo(string caminhoExcel, int numeroDeArquivosCriados, XLWorkbook novoArquivoExcel, DataTable dataTable, string novoDiretorio)
        {
            novoArquivoExcel.AddWorksheet(dataTable);

            if (!Directory.Exists(caminhoExcel))
                Directory.CreateDirectory(novoDiretorio);

            novoArquivoExcel.SaveAs(
                $"{novoDiretorio}\\novoExcelParte{numeroDeArquivosCriados + 1}.xlsx");

            Console.WriteLine($"Novo arquivo excel de número {numeroDeArquivosCriados + 1} criado! \n");

            return novoDiretorio;
        }

        private void MontarCabecalhoDoNovoArquivo(DataTable dataTable, IXLWorksheet worksheetOriginal)
        {
            IXLRow headerRow = worksheetOriginal.FirstRow();
            foreach (var item in headerRow.CellsUsed())
            {
                dataTable.Columns.Add(item.Value.ToString());
            }
        }

        private void VerificarSeArquivoExiste(string caminhoExcel, string nomeArquivo)
        {
            string caminhoCompleto = Path.Combine($"{caminhoExcel}\\{nomeArquivo}");
            if (!File.Exists(caminhoCompleto))
            {
                throw new FileNotFoundException();
            }
        }
    }
}
