using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Domain.Models;

namespace DBMigrationTool
{
    public static class CreatePortfolioTable_1
    {
        private const string PORTFOLIO_TABLE_NAME = "Portfolios";

        public static void Up(IAmazonDynamoDB client)
        {
            var currentTables = client.ListTablesAsync().Result;
            List<string> currentTableNames = currentTables.TableNames;
            if (currentTableNames.Contains(PORTFOLIO_TABLE_NAME)) return;

            var request = new CreateTableRequest
            {
                TableName = PORTFOLIO_TABLE_NAME,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = nameof(Portfolio.Id),
                        AttributeType = ScalarAttributeType.S
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = nameof(Portfolio.Id),
                        KeyType = KeyType.HASH //Partition key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            };

            var response = client.CreateTableAsync(request).Result;
        }

        public static void Down(IAmazonDynamoDB client)
        {
            List<string> currentTableNames = client.ListTablesAsync().Result.TableNames;
            if (!currentTableNames.Contains(PORTFOLIO_TABLE_NAME)) return;

            var response = client.DeleteTableAsync(PORTFOLIO_TABLE_NAME).Result;
        }
    }
}