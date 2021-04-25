using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Api.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDynamoDbClient(this IServiceCollection services, IConfiguration config)
        {
            var dynamoConfig = config.GetSection("DynamoDB");
            var isLocalMode = dynamoConfig.GetValue<bool>("LocalMode");

            if (isLocalMode)
            {
                services.AddSingleton<IAmazonDynamoDB>(sp =>
                {
                    var clientConfig = new AmazonDynamoDBConfig { ServiceURL = dynamoConfig.GetValue<string>("LocalServiceUrl") };
                    return new AmazonDynamoDBClient(clientConfig);
                });
            }
            else 
                services.AddAWSService<IAmazonDynamoDB>();

            services.AddScoped<IDynamoDBContext, DynamoDBContext>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped<IPortfolioRepository, PortfoliosRepository>();

            return services;
        }
    }
}