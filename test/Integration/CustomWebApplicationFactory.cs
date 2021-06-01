using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Api;
using Microsoft.Extensions.Configuration;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Extensions.CognitoAuthentication;
using Integration.Utilities;
using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using System.Threading.Tasks;
using System.Net.Http;

namespace Integration
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>, IDisposable
    {
        private IConfiguration Configuration;
        private CognitoTestConfig CognitoTestConfig;

        public CustomWebApplicationFactory()
        {
            /***
            * Gets the configuration from the appsettings.Testing.json placed in the Api folder.
            ***/
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Testing";
            Configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{env}.json", optional: false)
                .Build();

            CognitoTestConfig = GetCognitoTestConfig();
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
            .UseEnvironment("Testing")
            .ConfigureServices(services =>
            {

            });
        }

        private CognitoTestConfig GetCognitoTestConfig()
        {
            var cognitoTestConfig = Configuration.GetSection("Cognito")
                .Get<CognitoTestConfig>();

            return cognitoTestConfig;
        }

        public IAmazonDynamoDB GetDynamoDB()
        {
            var db = GetScopedService<IAmazonDynamoDB>();

            return db;
        }

        public IDynamoDBContext GetDynamoDBContext()
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

        public HttpClient CreateAuthorizedClient()
        {
            var client = base.CreateClient();
            var accessToken = GetTestUserToken().Result;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            return client;
        }

        public async Task<string> GetTestUserToken()
        {
            var user = GetTestUser(CognitoTestConfig);
            var authRequest = new InitiateSrpAuthRequest{Password = CognitoTestConfig.TestUserPassword};

            var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            var token = authResponse.AuthenticationResult.IdToken;

            return token;
        }

        private CognitoUser GetTestUser(CognitoTestConfig config)
        {            
            var provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials());
            var userPool = new CognitoUserPool(
                config.UserPoolId,
                config.ClientId,
                provider);
            var user = new CognitoUser(
                config.TestUsername,
                config.ClientId,
                userPool,
                provider);

            return user;
        }
    }
}