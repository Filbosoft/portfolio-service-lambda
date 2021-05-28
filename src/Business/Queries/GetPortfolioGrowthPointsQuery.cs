using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business.Wrappers;
using Conditus.Trader.Domain.Entities;
using Amazon.DynamoDBv2;
using System;
using Amazon.DynamoDBv2.Model;
using Business.Extensions;
using Conditus.DynamoDBMapper.Mappers;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Business.HelperMethods;

namespace Business.Queries
{
    public class GetPortfolioGrowthPointsQuery : BusinessRequest, IRequestWrapper<IEnumerable<PortfolioGrowthPoint>>
    {
        [Required]
        public string PortfolioId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetPortfolioGrowthPointsQueryHandler : IHandlerWrapper<GetPortfolioGrowthPointsQuery, IEnumerable<PortfolioGrowthPoint>>
    {
        private readonly IAmazonDynamoDB _db;

        public GetPortfolioGrowthPointsQueryHandler(IAmazonDynamoDB db)
        {
            _db = db;
        }

        public const double DAYS_IN_TEN_YEARS = 10*365.25;

        public async Task<BusinessResponse<IEnumerable<PortfolioGrowthPoint>>> Handle(GetPortfolioGrowthPointsQuery request, CancellationToken cancellationToken)
        {
            if (request.FromDate == null)
                request.FromDate = DateTime.UtcNow.AddMonths(-1);
            if (request.ToDate == null)
                request.ToDate = DateTime.UtcNow;

            if (request.FromDate > request.ToDate)
                return BusinessResponse.Fail<IEnumerable<PortfolioGrowthPoint>>("FromDate cannot be later than ToDate");
            
            var dateDiff = ((DateTime)request.ToDate) - ((DateTime)request.FromDate);
            if (dateDiff.TotalDays > DAYS_IN_TEN_YEARS)
                return BusinessResponse.Fail<IEnumerable<PortfolioGrowthPoint>>("Timespan must be less than 10 years");

            var query = GetQueryRequest(request);
            var response = await _db.QueryAsync(query);
            var entities = response.Items
                .Select(i => i.ToEntity<PortfolioGrowthPoint>())
                .ToList();

            var all = await _db.ScanAsync("PortfolioGrowthPoints",new List<string>{"PortfolioId","GrowthPointTimeStamp","Growth"});


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