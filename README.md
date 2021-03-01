# Processamento de arquivo excel em background - API
Projeto .NET que realiza um upload de um arquivo excel com muitos registros de forma assíncrona para posteriormente realizar a divisão do mesmo em arquivos menores.

## Tecnologias utilizadas

Para construir este projeto foram utilizadas seguintes tecnologias:

* Swashbuckle.AspNetCore para documentação da API.
* ClosedXML para trabalhar com arquivos excel.
* Hangfire.AspNetCore para processamento do arquivo excel em background.

## Inicialização do projeto
Para rodar este projeto basta executar os comandos na sua CLI de preferência estando dentro do contexto do arquivo "UploadArquivoAssincrono.sln" e seguir os passos abaixo:

```
$ dotnet clean
$ dotnet restore 
$ dotnet build 
$ dotnet run --project UploadArquivoAssincrono.API\UploadArquivoAssincrono.API.csproj
```

O projeto estará executando na url: ```http://localhost:5000```. Caso o projeto seja executado a partir do Visual Studio, o projeto será executado na url: ```http://localhost:62359/```
