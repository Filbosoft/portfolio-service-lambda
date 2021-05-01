using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Api;
using Microsoft.Extensions.Configuration;
using System.IO;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;

namespace Integration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>, IDisposable
    {
        private IConfiguration Configuration;
        public readonly IDynamoDBContext DbContext;        
        private readonly string _baseRequestString = File.ReadAllText("./SampleRequests/RequestBase.json");
        public CustomWebApplicationFactory()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing";
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            DbContext = GetScopedService<IDynamoDBContext>();
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment("Testing")
                .ConfigureAppConfiguration((ContextBoundObject, config) =>
                {
                    config
                        .AddConfiguration(Configuration);
                })
                .ConfigureServices(services =>
                {
                    services
                        .ReplaceServicesWithTestServices();
                });
        }

        // public HttpClient CreateAuthorizedClient()
        // {
        //     var client = base.CreateClient();
        //     var accessToken = Configuration.GetValue<string>("Authentication:testUserToken");
        //     client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        //     return client;
        // }
        public APIGatewayProxyRequest CreateBaseRequest()
        {
            var baseRequest = JsonSerializer.Deserialize<APIGatewayProxyRequest>(_baseRequestString, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return baseRequest;
        }

        public IDynamoDBContext GetDbContext() => DbContext;

        public T GetScopedService<T>()
        {            
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();

            return scope.ServiceProvider.GetService<T>();
        }
    }
}