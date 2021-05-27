using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Database
{
    public static class CreatePortfolioGrowthPointTable_2
    {
        private const string PORTFOLIO_GROWTH_POINTS_TABLE_NAME = "PortfolioGrowthPoints";

        public static void Up(IAmazonDynamoDB client)
        {
            var currentTables = client.ListTablesAsync().Result;
            List<string> currentTableNames = currentTables.TableNames;
            if (currentTableNames.Contains(PORTFOLIO_GROWTH_POINTS_TABLE_NAME)) return;
            Console.WriteLine($"Applying {nameof(CreatePortfolioGrowthPointTable_2)}");

            var partitionKey = new KeySchemaElement
            {
                AttributeName = "PortfolioId",
                KeyType = KeyType.HASH
            };

            var request = new CreateTableRequest
            {
                TableName = PORTFOLIO_GROWTH_POINTS_TABLE_NAME,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "PortfolioId",
                        AttributeType = ScalarAttributeType.S
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "GrowthPointTimestamp",
                        AttributeType = ScalarAttributeType.N
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    partitionKey,
                    new KeySchemaElement
                    {
                        AttributeName = "GrowthPointTimestamp",
                        KeyType = KeyType.RANGE
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 2
                }
            };

            var response = client.CreateTableAsync(request).Result;
        }

        public static void Down(IAmazonDynamoDB client)
        {
            List<string> currentTableNames = client.ListTablesAsync().Result.TableNames;
            if (!currentTableNames.Contains(PORTFOLIO_GROWTH_POINTS_TABLE_NAME)) return;
            Console.WriteLine($"Removing {nameof(CreatePortfolioGrowthPointTable_2)}");

            var response = client.DeleteTableAsync(PORTFOLIO_GROWTH_POINTS_TABLE_NAME).Result;
        }
    }
}