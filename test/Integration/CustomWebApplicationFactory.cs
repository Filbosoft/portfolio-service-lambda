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
using Amazon.DynamoDBv2;

namespace Integration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>, IDisposable
    {
        private IConfiguration Configuration;
        public readonly IDynamoDBContext DbContext;
        public readonly IAmazonDynamoDB Db;
        private readonly string _baseRequestString = File.ReadAllText("RequestBase.json");
        public CustomWebApplicationFactory()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing";
            Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            Db = GetScopedService<IAmazonDynamoDB>();
            DbContext = GetScopedService<IDynamoDBContext>();
        }

        public new void Dispose()
        {
            Db.Dispose();
            DbContext.Dispose();
            Server.Dispose();
            
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

        public IAmazonDynamoDB GetDb() => Db;
        public IDynamoDBContext GetDbContext() => DbContext;

        public T GetScopedService<T>()
        {            
            var scopeFactory = this.Services.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();

            return scope.ServiceProvider.GetService<T>();
        }
    }
}