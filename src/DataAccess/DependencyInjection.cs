﻿using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessDependencies(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddDynamoDbClient(config)
                .AddRepositories();

            return services;
        }

        private static IServiceCollection AddDynamoDbClient(this IServiceCollection services, IConfiguration config)
        {
            var dynamoConfig = config.GetSection("DynamoDB");
            var isLocalMode = dynamoConfig.GetValue<bool>("LocalMode");

            if (isLocalMode)
            {
                services.AddScoped<IAmazonDynamoDB>(sp =>
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

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {

            return services;
        }
    }
}
