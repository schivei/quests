using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Dgraph4Net;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quests.DomainModels;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using System.Reflection;
using Newtonsoft.Json;
using Quests.Repositories.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Quests.Api
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
            RunDgraph(services);

            services.AddStores();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.AllowInputFormatterExceptionMessages = true;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Quests API",
                    Version = "v1",
                    License = new OpenApiLicense
                    {
                        Name = "MIT"
                    },
                    Contact = new OpenApiContact
                    {
                        Email = "costa@elton.schivei.nom.br",
                        Name = "Elton Schivei Costa",
                        Url = new Uri("http://elton.schivei.nom.br")
                    }
                });
                c.MapType<Uid>(() => new OpenApiSchema
                {
                    Type = "uid",
                    Nullable = false,
                    Example = new OpenApiString(Uid.NewUid()),
                    Pattern = "^(0x|_:)([a-z0-9]+)$"
                });
                c.SchemaFilter<PropertiesSchemaFilter>();
                c.SchemaFilter<EnumSchemaFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Quest API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private enum OSType
        {
            Windows,
            Darwin,
            Linux
        }

        private OSType GetPlatform() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                OSType.Windows :
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
                OSType.Darwin :
                OSType.Linux;

        private void Map(IServiceCollection services, int tries = 0)
        {
            try
            {
                var grpc = new Channel(Configuration.GetConnectionString("dgraph"), ChannelCredentials.Insecure);

                var client = new Dgraph4NetClient(grpc);

                client.Map(typeof(Question).Assembly);

                services.AddSingleton(client);
            }
            catch
            {
                if (tries++ < 3)
                {
                    Thread.Sleep(2000);
                    Map(services, tries);
                    return;
                }

                throw;
            }
        }

        public static List<Process> Processes { get; } = new List<Process>();

        private void StartDgraph(string command, bool isTest)
        {
            var di = new DirectoryInfo("AppData");
            var fi = new FileInfo(command);

            if (!di.Exists)
                di.Create();

            var p1 =
            Process.Start(new ProcessStartInfo
            {
                FileName = fi.FullName,
                WorkingDirectory = di.FullName,
                Arguments = "alpha --lru_mb 1024",
                CreateNoWindow = GetPlatform() != OSType.Windows,
                UseShellExecute = GetPlatform() == OSType.Windows,
                WindowStyle = ProcessWindowStyle.Normal
            });

            var p2 =
            Process.Start(new ProcessStartInfo
            {
                FileName = fi.FullName,
                WorkingDirectory = di.FullName,
                Arguments = "zero",
                CreateNoWindow = GetPlatform() != OSType.Windows,
                UseShellExecute = GetPlatform() == OSType.Windows,
                WindowStyle = ProcessWindowStyle.Normal
            });

            if (isTest)
            {
                Processes.Add(p1);
                Processes.Add(p2);
            }
        }

        private void RunDgraph(IServiceCollection services)
        {
            var platform = GetPlatform().ToString().ToLowerInvariant();
            var dgraphLocation = Path.Combine("..", "..", "dgraph");

            var env =
            services.FirstOrDefault(s => s.ImplementationInstance is IWebHostEnvironment)?
                .ImplementationInstance as IWebHostEnvironment;

            var isTest = env?.EnvironmentName == "Tests" ||
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Tests";

            if (isTest)
                dgraphLocation = Path.Combine("..", "..", "..", "..", "..", "dgraph");

            if (!Directory.Exists(dgraphLocation))
                Directory.CreateDirectory(dgraphLocation);

            var executable = Path.Combine(dgraphLocation, $"dgraph{(platform != "windows" ? "" : ".exe")}");

            if (File.Exists(executable))
            {
                var procs = Process.GetProcessesByName("dgraph");
                if (procs.Length < 2)
                {
                    foreach (var proc in procs)
                    {
                        proc.Kill(true);
                    }

                    StartDgraph(executable, isTest);
                }

                Map(services);
                return;
            }

            var link = $"https://github.com/dgraph-io/dgraph/releases/download/v20.03.0/dgraph-{platform}-amd64.tar.gz";

            using var wc = new WebClient();

            try
            {
                wc.DownloadFile(link, Path.Combine(dgraphLocation, $"dgraph-{platform}-amd64.tar.gz"));

                Path.Combine(dgraphLocation, $"dgraph-{platform}-amd64.tar.gz").ExtractTarGz(dgraphLocation);
            }
            catch (Exception e)
            {
                throw new ExternalException("Não foi possível baixar o banco de dados.", e);
            }

            StartDgraph(executable, isTest);

            Map(services);
        }
    }
}
