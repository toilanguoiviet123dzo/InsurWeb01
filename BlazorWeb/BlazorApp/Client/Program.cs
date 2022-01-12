using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;
using Cores.GrpcClient.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Cores.SystemConfig.Services;
using Cores.Admin.Services;
using Blazored.Toast;
using Cores.Compensation.Services;
using Cores.Resource.Services;
using BlazorApp.Client.Services;

namespace BlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //grpc Web
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            //Http client
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //gRpc
            builder.Services.AddMyServices();

            //DevExpress blazor
            builder.Services.AddDevExpressBlazor();

            //Blazored Toast
            builder.Services.AddBlazoredToast();

            await builder.Build().RunAsync();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            //Create grpc channel
            var baseUri = services.BuildServiceProvider().GetRequiredService<NavigationManager>().BaseUri;
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
            var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });

            // grpcService
            services.AddSingleton(services => new grpcAdminService.grpcAdminServiceClient(channel));
            services.AddSingleton(services => new grpcCompensationService.grpcCompensationServiceClient(channel));
            services.AddSingleton(services => new grpcResourceService.grpcResourceServiceClient(channel));

            //Authentication
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

            //Setting master
            services.AddSingleton<SettingService>();
            services.AddSingleton<MasterService>();

            //
            return services;
        }
    }
}
