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
        private IDynamoDBContext DBContext;

        public CustomWebApplicationFactory()
        {
            /***
            * Gets the configuration from the appsettings.json placed in the Api/conf folder.
            ***/
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing";
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            builder
            .UseEnvironment("Testing")
            .ConfigureAppConfiguration((ContextBoundObject, config) =>
            {
                config
                    .AddJsonFile(configPath);
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
            var baseRequestStr = File.ReadAllText("./SampleRequests/RequestBase.json");
            var baseRequest = JsonSerializer.Deserialize<APIGatewayProxyRequest>(baseRequestStr, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return baseRequest;
        }

        public IDynamoDBContext GetDbContext()
        {
            var dbContext = GetScopedService<IDynamoDBContext>();

            return dbContext;
        }

        public T GetScopedService<T>()
        {            
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();

            return scope.ServiceProvider.GetService<T>();
        }
    }
}