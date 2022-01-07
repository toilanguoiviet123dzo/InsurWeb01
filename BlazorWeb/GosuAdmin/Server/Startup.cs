using Gosu.Configs;
using Gosu.Grpc.Client;
using GosuAdmin.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Gosu.Grpc.Authentication;
using System;

namespace GosuAdmin.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public async void ConfigureServices(IServiceCollection services)
        {
            // add gRPC service 
            services.AddGrpc();
            services.AddGrpc(options =>
            {
                // This registers a global auth interceptor
                options.Interceptors.Add<AuthInterceptor>();
            });
            //Cors
            services.AddCors(o => o.AddPolicy(MyAllowSpecificOrigins, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));
            
            //services.AddControllersWithViews();
            //services.AddRazorPages();

            //Init grpc client
            int maxChannelCount = 5;
            string systemConfigUrl = "http://222.253.79.223:5099";
            var grpcConfig = Configuration.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            if (grpcConfig != null && grpcConfig.MaxChannelCount > 0)
            {
                maxChannelCount = grpcConfig.MaxChannelCount;
                systemConfigUrl = grpcConfig.SystemConfigUrl;
            }
            await GrpcClientFactory.InitAsync(maxChannelCount, systemConfigUrl);

            //Gosu service
            services.AddGosuServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseGrpcWeb();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                // map to and register the gRPC service
                //
                endpoints.MapGrpcService<AdminService>().EnableGrpcWeb().RequireCors(MyAllowSpecificOrigins);
                endpoints.MapGrpcService<CompensationService>().EnableGrpcWeb().RequireCors(MyAllowSpecificOrigins);
                endpoints.MapGrpcService<ResourceService>().EnableGrpcWeb().RequireCors(MyAllowSpecificOrigins);
                //
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGosuServices(this IServiceCollection services)
        {
            //Init authentication
            AuthenticationService.Init();


            //Service list
            //services.AddScoped<IUserService, UserService>();

            //....
            //
            return services;
        }
    }

}//End namespace
