using ClosedXML.Excel;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace DivisaoArquivoExcel
{

    class Program
    {
        private static readonly int LINHAS_MAXIMA_POR_ARQUIVO = 1000;
        private static readonly string EXTENSAO_ARQUIVO = ".xlsx";
        private static readonly string CAMINHO_NOVO_ARQUIVO = "C:\\projetos\\upload-arquivo-assincrono\\backend\\UploadArquivoAssincrono.API\\Upload\\files\\";
        private static readonly string NOVA_PASTA = "excels-para-processamento";

        static void Main(string[] args)
        {

            Console.WriteLine($"Iniciando processamento/quebra do arquivo excel informado!");
            Console.WriteLine($"---------------------------------------------------------- \n");
            
            string localizacaoDoArquivo = "C:\\projetos\\upload-arquivo-assincrono\\backend\\UploadArquivoAssincrono.API\\Upload\\files";
            string nomeArquivo = $"guid{EXTENSAO_ARQUIVO}";

            Console.WriteLine($"Iniciando quebra do arquivo {nomeArquivo}");
            Console.WriteLine($"---------------------------------------------------------- \n");

            try
            {
                DividirExcel(localizacaoDoArquivo, nomeArquivo);
            } catch (Exception ex)
            {
                Console.WriteLine("Ocorreu um erro ao realizar o processamento do excel. \n");
                Console.WriteLine($"{ex.InnerException}\n");
            }
            
        }

        static void DividirExcel(string caminhoExcel, string nomeArquivo)
        {
            // Procurar arquivo excel
            VerificarSeArquivoExiste(caminhoExcel, nomeArquivo);

            // Transformar arquivo excel em Workbook
            using (XLWorkbook arquivoExcel = new XLWorkbook($"{caminhoExcel}\\{nomeArquivo}")) 
            {
                int numeroDeWorksheets = arquivoExcel.Worksheets.Count;
                if (numeroDeWorksheets <= 0)
                    return;

                var worksheetOriginal = arquivoExcel.Worksheets.FirstOrDefault();
                int totalDeLinhasNoArquivo = worksheetOriginal.RowsUsed().Count();
                Console.WriteLine($"Arquivo Excel possui {totalDeLinhasNoArquivo} no total!\n");

                int numeroDeArquivosNoTotal = (totalDeLinhasNoArquivo - 1) / LINHAS_MAXIMA_POR_ARQUIVO;
                Console.WriteLine($"Serão criados {numeroDeArquivosNoTotal} arquivos excel no total! \n");

                bool linhaDoCabecalho = true;
                for (int numeroDoNovoArquivo = 0; numeroDoNovoArquivo < numeroDeArquivosNoTotal; numeroDoNovoArquivo++)
                {
                    XLWorkbook novoArquivoExcel = new XLWorkbook();
                    DataTable dataTable = new DataTable($"ExcelGerado-{numeroDoNovoArquivo + 1}-{nomeArquivo}");

                    // Cria cabeçalho do novo arquivo
                    MontarCabecalhoDoNovoArquivo(dataTable, worksheetOriginal);
                    Console.WriteLine($"Montado cabeçalho do arquivo número {numeroDoNovoArquivo + 1} \n");

                    int linhasProcessadas = 0;
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
                        int colunaAtual = 0;
                        foreach (var item in linha.CellsUsed())
                        {
                            dataTable.Rows[dataTable.Rows.Count - 1][colunaAtual] = item.Value.ToString();
                            colunaAtual++;
                        }
                        linhasProcessadas++;
                    }

                    SalvarNovoArquivo(caminhoExcel, numeroDoNovoArquivo, novoArquivoExcel, dataTable);
                }
            }
        }

        private static void SalvarNovoArquivo(string caminhoExcel, int numeroDeArquivosCriados, XLWorkbook novoArquivoExcel, DataTable dataTable)
        {
            string novoDiretorio = $"{CAMINHO_NOVO_ARQUIVO}{NOVA_PASTA}-{DateTime.Now.Date.ToString("dd-MM-yyyy")}";

            novoArquivoExcel.AddWorksheet(dataTable);

            if (!Directory.Exists(caminhoExcel))
                Directory.CreateDirectory(novoDiretorio);

            novoArquivoExcel.SaveAs(
                $"{novoDiretorio}\\novoExcelParte{numeroDeArquivosCriados + 1}.xlsx");

            Console.WriteLine($"Novo arquivo excel de número {numeroDeArquivosCriados + 1} criado! \n");
        }

        private static void MontarCabecalhoDoNovoArquivo(DataTable dataTable, IXLWorksheet worksheetOriginal)
        {
            IXLRow headerRow = worksheetOriginal.FirstRow();
            foreach (var item in headerRow.CellsUsed())
            {
                dataTable.Columns.Add(item.Value.ToString());
            }
        }

        private static void VerificarSeArquivoExiste(string caminhoExcel, string nomeArquivo)
        {
            string caminhoCompleto = Path.Combine($"{caminhoExcel}\\{nomeArquivo}");
            if (!File.Exists(caminhoCompleto))
            {
                throw new FileNotFoundException();
            }
        }
    }

    
}
