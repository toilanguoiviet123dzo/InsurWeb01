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
using Gosu.SystemConfig.Services;
using Gosu.Configs;
using MongoDB.Entities;
using MongoDB.Driver;
using Gosu.Common;
using System.Security;
using System.Text;
using System.Net.Http;
using System.Threading;

namespace Gosu.Service
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
        public async void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            //Configs
            services.Configure<DatabaseConfig>(Configuration.GetSection("DatabaseConfig"));
            var databaseConfig = Configuration.GetSection(nameof(DatabaseConfig)).Get<DatabaseConfig>();

            //Mongle DB
            await DB.InitAsync(databaseConfig.DBName,
                MongoClientSettings.FromConnectionString(databaseConfig.ConnectionString));

            ////Init grpc client            
            //int maxChannelCount = 5;
            //string systemConfigUrl = "http://123.30.106.114:5099";
            //var grpcConfig = Configuration.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            //if (grpcConfig != null && grpcConfig.MaxChannelCount > 0)
            //{
            //    maxChannelCount = grpcConfig.MaxChannelCount;
            //    systemConfigUrl = grpcConfig.SystemConfigUrl;
            //}
            //await GrpcClientFactory.InitAsync(maxChannelCount, systemConfigUrl);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //Grpc
                endpoints.MapGrpcService<SystemConfigService>();                
                //
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
