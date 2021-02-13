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
        private ILogger<ProcessamentoExcelService> logger;

        public ProcessamentoExcelService(ILogger<ProcessamentoExcelService> logger)
        {
            this.logger = logger;
        }

        public async Task<PSDataCollection<PSObject>> InvokeScript(string script) 
        {
            try {
                using (PowerShell ps = PowerShell.Create()) 
                {
                    ps.AddScript(script);

                    var outputCollection = new PSDataCollection<PSObject>();
                    outputCollection.DataAdded += OutputCollection_DataAdded;
                    ps.Streams.Error.DataAdded += Error_DataAdded;

                    IAsyncResult result = await Task.Run(() => ps.BeginInvoke<PSObject, PSObject>(null, outputCollection));

                    while (result.IsCompleted == false) 
                    {
                        this.logger.LogInformation("Executing...");
                        Thread.Sleep(100);
                    }

                    foreach (var item in ps.Streams.Error)
                    {
                        this.logger.LogError(item.ToString());
                    }

                    this.logger.LogInformation("Execution has stopped. Execution state: " + ps.InvocationStateInfo.State);

                    return outputCollection;
                }
            } catch (Exception ex) {
                throw ex;
            }
            
        }

        private void OutputCollection_DataAdded(object sender, DataAddedEventArgs e) 
        {
            this.logger.LogInformation("Object added to output");
        }

        private void Error_DataAdded(object sender, DataAddedEventArgs e) 
        {
            this.logger.LogInformation("An error war written to the error stream");
        }

        // public async System.Threading.Tasks.Task ProcessarArquivoExcelAsync()
        // {
        //     string diretorioAtual = Directory.GetCurrentDirectory();
        //     string pathBuilt = Path.Combine(Directory.GetParent(diretorioAtual).FullName, "UploadArquivoAssincrono.API\\Upload\\files");

        //     if (!Directory.Exists(pathBuilt))
        //         return;


        //     var scriptContents = new StringBuilder();
        //     scriptContents.AppendLine("$r=@()");
        //     scriptContents.AppendLine("$t=$C=1");
        //     scriptContents.AppendLine("Import-Excel -Path C:\\estudos-angular\\TESTE\\guid.xlsx|Foreach-Object -Process {");
        //     scriptContents.AppendLine("$r += $_");
        //     scriptContents.AppendLine("");
        //     scriptContents.AppendLine("if($C -eq 2000){");
        //     scriptContents.AppendLine("$r | Export-Excel -Path C:\\estudos-angular\\TESTE\test_$t.xlsx");
        //     scriptContents.AppendLine("");
        //     scriptContents.AppendLine("$r=@()");
        //     scriptContents.AppendLine("$c=1");
        //     scriptContents.AppendLine("$t++");
        //     scriptContents.AppendLine("}");
        //     scriptContents.AppendLine("else{");
        //     scriptContents.AppendLine("$c++");
        //     scriptContents.AppendLine("}");
        //     scriptContents.AppendLine("}");
        //     scriptContents.AppendLine("");
        //     scriptContents.AppendLine("$r|Export-Excel -Path C:\\estudos-angular\\TESTE\\test_$t.xlsx");
        //     scriptContents.AppendLine("}");



        //     HostedRunspace hosted = new HostedRunspace();
        //     hosted.InitializeRunspaces(2, 10, new string[] { "ImportExcel" });

        //     Console.WriteLine("Calling RunScript()");
        //     await hosted.RunScript(scriptContents.ToString());
        //     //// Procurar arquivo excel


        //     //string[] arquivosNoDiretorio = Directory.GetFiles(pathBuilt);
        //     //if (arquivosNoDiretorio == null || arquivosNoDiretorio.Length == 0)
        //     //    return;

        //     //string caminhoArquivo = arquivosNoDiretorio[0];


        //     //string novoDiretorio = $"{pathBuilt}\\excels-para-processamento-{DateTime.Now.Date.ToString("dd-MM-yy-HH-mm-ss")}";
        //     //// Transformar arquivo excel em Workbook
        //     //using (XLWorkbook arquivoExcel = new XLWorkbook(caminhoArquivo))
        //     //{
        //     //    int numeroDeWorksheets = arquivoExcel.Worksheets.Count;
        //     //    if (numeroDeWorksheets <= 0)
        //     //        return;

        //     //    var worksheet = arquivoExcel.Worksheets.FirstOrDefault(); ;
        //     //    int totalDeLinhas = worksheet.RowsUsed().Count();
        //     //    int linhasMaximaPorArquivo = 500;
        //     //    int linhasPorNovoArquivo = linhasMaximaPorArquivo;

        //     //    if (linhasMaximaPorArquivo > totalDeLinhas )
        //     //    {
        //     //        linhasPorNovoArquivo = totalDeLinhas - 1;
        //     //    }

        //     //    int ultimaPrimeiraLinha = 1;
        //     //    int contador = 1;
        //     //    do
        //     //    {
        //     //        var primeraColuna = worksheet.Rows(ultimaPrimeiraLinha, linhasPorNovoArquivo + ultimaPrimeiraLinha).First().FirstCell().Address;
        //     //        var ultimaColuna = worksheet.Rows(ultimaPrimeiraLinha, linhasPorNovoArquivo).Last().LastCellUsed().Address;

        //     //        var range = worksheet.Range(primeraColuna, ultimaColuna).AsRange(); //.RangeUsed();
        //     //        var table = range.AsTable();

        //     //        var dataList = new List<string[]>
        //     //        {
        //     //            table.DataRange.Rows()
        //     //                .Select(tableRow => tableRow.Cell(1).Value.ToString())
        //     //                .ToArray(),

        //     //            table.DataRange.Rows()
        //     //                .Select(tableRow => tableRow.Cell(2).Value.ToString())
        //     //                .ToArray()
        //     //        };

        //     //        //Convert List to DataTable
        //     //        var dataTable = ConvertListToDataTable(dataList);


        //     //        XLWorkbook novoExcelParte1 = new XLWorkbook();
        //     //        //XLWorkbook novoExcelParte2 = new XLWorkbook();
        //     //        novoExcelParte1.AddWorksheet(dataTable);


        //     //        if (!Directory.Exists(novoDiretorio)) {
        //     //            Directory.CreateDirectory(novoDiretorio);
        //     //        }

        //     //        novoExcelParte1.SaveAs($"{novoDiretorio}\\novoExcelParte{contador}.xlsx");

        //     //        ultimaPrimeiraLinha = ultimaPrimeiraLinha + linhasPorNovoArquivo;
        //     //        contador++;
        //     //    } while (ultimaPrimeiraLinha < totalDeLinhas);

        //     //        //var primeraColuna = worksheet.Row(worksheet.FirstRowUsed().RowNumber()).FirstCell().Address;
        //     //    //var ultimaColuna = worksheet.LastCellUsed().Address;






        //     //    //int linhaFinal = totalDeLinhas - 1;
        //     //    //int contador = 1;

        //     //    //int novaPrimeiraLinha = 0;
        //     //    //int novaUltimaLinha = metadeDeLinhasDoArquivo;
        //     //    //while (linhaFinal <= totalDeLinhas)
        //     //    //{
        //     //    //    if (novaPrimeiraLinha = 0)
        //     //    //        var novoArquivoExcel = new XLWorkbook();
        //     //    //    var novoWorksheet = novoArquivoExcel.Worksheets.Add($"arquivoExcel-parte-{contador + 1}");


        //     //    //    foreach (var linha in worksheet.Rows(novaPrimeiraLinha, novaUltimaLinha))
        //     //    //    {
        //     //    //        foreach (var coluna in linha.Cells(true))
        //     //    //        {
        //     //    //            novoWorksheet.Cell(coluna.Address).Value = coluna.Value;
        //     //    //        }
        //     //    //    }



        //     //    //}
        //     //    //for (int i = 0; i < metadeDeLinhasDoArquivo; i++)
        //     //    //{




        //     //    //}

        //     //}







        //     //// Dividir arquivo excel em 2

        //     //// Salvar os 2 arquivos excel

        //     //// Ler o primeiro

        //     //// Ler o segundo

        //     //// Finalizar
        //     //if (File.Exists(caminhoArquivo))
        //     //{
        //     //    File.Move(caminhoArquivo, $"{novoDiretorio}\\_arquivoOriginal.xlsx");
        //     //    File.SetAttributes($"{novoDiretorio}\\_arquivoOriginal.xlsx", FileAttributes.ReadOnly);
        //     //}

        //     //return;
        // }

        // private static DataTable ConvertListToDataTable(IReadOnlyList<string[]> list)
        // {
        //     var table = new DataTable("NovoExcel");
        //     var rows = list.Select(array => array.Length).Concat(new[] { 0 }).Max();

        //     table.Columns.Add("NOME");
        //     table.Columns.Add("IDADE");

        //     for (var j = 0; j < rows; j++)
        //     {
        //         var row = table.NewRow();
        //         row["NOME"] = list[0][j];
        //         row["IDADE"] = list[1][j];
        //         table.Rows.Add(row);
        //     }
        //     return table;
        // }

    }
}

