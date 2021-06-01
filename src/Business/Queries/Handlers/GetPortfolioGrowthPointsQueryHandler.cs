using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Wrappers;
using Conditus.Trader.Domain.Entities;
using Amazon.DynamoDBv2;
using System;
using Amazon.DynamoDBv2.Model;
using Conditus.DynamoDBMapper.Mappers;
using System.Linq;
using Business.HelperMethods;

namespace Business.Queries.Handlers
{
    public class GetPortfolioGrowthPointsQueryHandler : IHandlerWrapper<GetPortfolioGrowthPointsQuery, IEnumerable<PortfolioGrowthPoint>>
    {
        private readonly IAmazonDynamoDB _db;

        public GetPortfolioGrowthPointsQueryHandler(IAmazonDynamoDB db)
        {
            _db = db;
        }

        public const double MAX_TIMESPAN_IN_YEARS = 10;
        public const double MAX_TIMESPAN_IN_DAYS = MAX_TIMESPAN_IN_YEARS*365.25;

        public async Task<BusinessResponse<IEnumerable<PortfolioGrowthPoint>>> Handle(GetPortfolioGrowthPointsQuery request, CancellationToken cancellationToken)
        {
            if (request.FromDate == null)
                request.FromDate = DateTime.UtcNow.AddMonths(-1);
            if (request.ToDate == null)
                request.ToDate = DateTime.UtcNow;

            if (request.FromDate > request.ToDate)
                return BusinessResponse.Fail<IEnumerable<PortfolioGrowthPoint>>(
                    GetPortfolioGrowthPointsResponseCodes.FromDateLaterThanToDate,
                    "FromDate cannot be after ToDate");
            
            var dateDiff = ((DateTime)request.ToDate) - ((DateTime)request.FromDate);
            if (dateDiff.TotalDays > MAX_TIMESPAN_IN_DAYS)
                return BusinessResponse.Fail<IEnumerable<PortfolioGrowthPoint>>(
                    GetPortfolioGrowthPointsResponseCodes.TimespanToLong,
                    $"Timespan must be less than {MAX_TIMESPAN_IN_YEARS} years");

            var query = GetQueryRequest(request);
            var response = await _db.QueryAsync(query);
            var entities = response.Items
                .Select(i => i.ToEntity<PortfolioGrowthPoint>())
                .ToList();

            return BusinessResponse.Ok<IEnumerable<PortfolioGrowthPoint>>(entities);
        }

        /***
        * Query parameters
        ***/
        private const string V_FROM_DATE = ":v_from_date";
        private const string V_TO_DATE = ":v_to_date";
        private const string V_PORTFOLIO_ID = ":v_portfolio_id";
        private const string V_REQUESTING_USER_ID = ":v_requesting_user_id";

        private QueryRequest GetQueryRequest(GetPortfolioGrowthPointsQuery request)
        {
            var query = new QueryRequest
            {
                TableName = DynamoDBHelper.GetDynamoDBTableName<PortfolioGrowthPoint>(),
                Select = "ALL_ATTRIBUTES",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {V_FROM_DATE, ((DateTime) request.FromDate).GetAttributeValue()},
                    {V_TO_DATE, ((DateTime) request.ToDate).GetAttributeValue()},
                    {V_REQUESTING_USER_ID, new AttributeValue { S = request.RequestingUserId }},
                    {V_PORTFOLIO_ID, new AttributeValue { S = request.PortfolioId }}
                }
            };

            SetQueryConditions(query, request);

            return query;
        }

        private void SetQueryConditions(QueryRequest query, GetPortfolioGrowthPointsQuery request)
        {
            var keyConditions = new List<string>
            {
                $"({nameof(PortfolioGrowthPoint.PortfolioId)} = {V_PORTFOLIO_ID})",
                $"({nameof(PortfolioGrowthPoint.GrowthPointTimestamp)} BETWEEN {V_FROM_DATE} AND {V_TO_DATE})"
            };

            query.KeyConditionExpression = string.Join(" AND ", keyConditions);
            query.FilterExpression = $"({nameof(PortfolioGrowthPoint.OwnerId)} = {V_REQUESTING_USER_ID})";
        }
    }
}