using System;
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
        protected readonly IDynamoDBContext _db;
        public TestsBase(CustomWebApplicationFactory<Startup> factory)
        {
            _db = factory.GetDbContext();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}