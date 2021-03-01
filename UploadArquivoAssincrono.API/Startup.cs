using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UploadArquivoAssincrono.API.BackgroundServices;
using UploadArquivoAssincrono.API.BackgroundServices.Interfaces;
using UploadArquivoAssincrono.API.ImportacaoExcel;

namespace UploadArquivoAssincrono.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    SchemaName = "Hangfire"
                }));

            services.AddControllers();
            services.AddCors();

            #region [Swagger Gen Configuration]

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Upload de arquivo assíncrono - API",
                    Description = "Um projeto .NET Core para upload de arquivos com muitos registros que serão divididos em arquivos menores para posterior processamento utilizando Hangfire",
                    TermsOfService = new Uri("https://github.com/sathoril/upload-arquivo-assincrono"),
                    Contact = new OpenApiContact
                    {
                        Name = "Bruno Fernando Corrêa de Abreu",
                        Email = "bruno.feabreu@gmail.com",
                        Url = new Uri("https://twitter.com/sathozin"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }
                });
            });

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(builder =>
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin());
            }

            #region [Swagger Configuration]

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Upload assícrono - API");
                c.RoutePrefix = string.Empty;
            });

            #endregion

            app.UseRouting();

            #region [Hangfire configuration]

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            #endregion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
