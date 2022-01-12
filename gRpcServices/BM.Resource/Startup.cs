using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Cores.Resource.Services;
using Cores.Configs;
using MongoDB.Entities;
using MongoDB.Driver;
using Cores.Common;
using System.Security;
using Cores.Extentions;
using System.Text;
using System.Net.Http;
using System.Threading;
using Cores.Service.Services;

namespace Cores.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //gRpc
            services.AddGrpc();
            services.AddSingleton<ResourceService>();

            //Configs
            var databaseConfig = Configuration.GetSection(nameof(DatabaseConfig)).Get<DatabaseConfig>();

            //Mongle DB
            Action initDBTask = new Action(async () =>
            {
                await DB.InitAsync(databaseConfig.DBName, MongoClientSettings.FromConnectionString(databaseConfig.ConnectionString));
            });

            //gRpc client
            Action initgRpcTask = new Action(async () =>
            {
                int maxChannelCount = 5;
                string systemConfigUrl = "http://123.30.106.114:5002";
                var grpcConfig = Configuration.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
                if (grpcConfig != null && grpcConfig.MaxChannelCount > 0)
                {
                    maxChannelCount = grpcConfig.MaxChannelCount;
                    systemConfigUrl = grpcConfig.SystemConfigUrl;
                }
                await GrpcClientFactory.InitAsync(maxChannelCount, systemConfigUrl);
            });

            //Run all task
            bool IsAllDone = false;
            TaskHelper.RunAsync(
                                () => IsAllDone = true,
                                ex => Console.WriteLine(ex),
                                initDBTask,
                                initgRpcTask);

            //Waiting for all task completed
            while (!IsAllDone)
            {
                Thread.Sleep(1000);
            }
            //
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //Grpc
                endpoints.MapGrpcService<ResourceService>();                
                //
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
