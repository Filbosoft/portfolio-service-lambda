using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Database.Indexes;

namespace Database
{
    public static class CreatePortfolioTable_1
    {
        private const string PORTFOLIO_TABLE_NAME = "Portfolios";

        public static void Up(IAmazonDynamoDB client)
        {
            var currentTables = client.ListTablesAsync().Result;
            List<string> currentTableNames = currentTables.TableNames;
            if (currentTableNames.Contains(PORTFOLIO_TABLE_NAME)) return;
            Console.WriteLine($"Applying {nameof(CreatePortfolioTable_1)}");

            var partitionKey = new KeySchemaElement
            {
                AttributeName = "OwnerId",
                KeyType = KeyType.HASH
            };

            var request = new CreateTableRequest
            {
                TableName = PORTFOLIO_TABLE_NAME,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = ScalarAttributeType.S
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "Name",
                        AttributeType = ScalarAttributeType.S
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "OwnerId",
                        AttributeType = ScalarAttributeType.S
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "CreatedAt",
                        AttributeType = ScalarAttributeType.N
                    }
                },
                KeySchema = new List<KeySchemaElement>()
                {
                    partitionKey,
                    new KeySchemaElement
                    {
                        AttributeName = "CreatedAt",
                        KeyType = KeyType.RANGE
                    }
                },
                LocalSecondaryIndexes = new List<LocalSecondaryIndex>
                {
                    new LocalSecondaryIndex
                    {
                        IndexName = LocalIndexes.PortfolioIdIndex,
                        KeySchema = new List<KeySchemaElement>
                        {
                            partitionKey,
                            new KeySchemaElement
                            {
                                AttributeName = "Id",
                                KeyType = KeyType.RANGE
                            }
                        },
                        Projection = new Projection{ProjectionType = ProjectionType.ALL}
                    },
                    new LocalSecondaryIndex
                    {
                        IndexName = LocalIndexes.PortfolioNameIndex,
                        KeySchema = new List<KeySchemaElement>
                        {
                            partitionKey,
                            new KeySchemaElement
                            {
                                AttributeName = "Name",
                                KeyType = KeyType.RANGE
                            }
                        },
                        Projection = new Projection{ProjectionType = ProjectionType.ALL}
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
            if (!currentTableNames.Contains(PORTFOLIO_TABLE_NAME)) return;
            Console.WriteLine($"Removing {nameof(CreatePortfolioTable_1)}");

            var response = client.DeleteTableAsync(PORTFOLIO_TABLE_NAME).Result;
        }
    }
}