using System;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.TestUtilities;
using Api;
using Xunit;

namespace Integration.Utilities
{
    public class TestsBase : IClassFixture<CustomWebApplicationFactory<Startup>>, IDisposable
    {
        protected readonly LambdaEntryPoint _entryPoint = new LambdaEntryPoint();
        protected readonly TestLambdaContext _context = new TestLambdaContext();
        protected readonly IAmazonDynamoDB _db;
        protected readonly IDynamoDBContext _dbContext;
        protected readonly CustomWebApplicationFactory<Startup> _factory;

        public TestsBase(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _db = factory.GetDb();
            _dbContext = factory.GetDbContext();
        }

        public void Dispose()
        {
            _db.Dispose();
            _dbContext.Dispose();
            _factory.Dispose();
        }
    }
}