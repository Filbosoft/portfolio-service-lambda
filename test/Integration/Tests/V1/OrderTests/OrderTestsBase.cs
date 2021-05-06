using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Api;
using Business.Commands;
using Business.Commands.PortfolioCommands;
using Domain.Models;
using FluentAssertions;
using FluentAssertions.Execution;
using Integration.Utilities;
using Xunit;

namespace Integration.Tests.V1.OrderTests
{
    public class OrderTestsBase : TestsBase
    {
        protected const string ORDER_URI = "api/v1/portfolios";

        public OrderTestsBase(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }
    }
}