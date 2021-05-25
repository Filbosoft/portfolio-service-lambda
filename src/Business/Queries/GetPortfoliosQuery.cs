using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using Business.Wrappers;
using Conditus.Trader.Domain.Models;
using Conditus.Trader.Domain.Entities;
using Amazon.DynamoDBv2;
using System;
using Amazon.DynamoDBv2.Model;
using Business.Extensions;
using Conditus.DynamoDBMapper.Mappers;
using Database.Indexes;
using System.Linq;

namespace Business.Queries
{
    public class GetPortfoliosQuery : BusinessRequest, IRequestWrapper<IEnumerable<PortfolioOverview>>
    {
        public string NameQuery { get; set; }
        public DateTime? CreatedFromDate { get; set; }
    }

    public class GetPortfoliosQueryHandler : IHandlerWrapper<GetPortfoliosQuery, IEnumerable<PortfolioOverview>>
    {
        private readonly IAmazonDynamoDB _db;
        private readonly IMapper _mapper;

        public GetPortfoliosQueryHandler(IAmazonDynamoDB db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BusinessResponse<IEnumerable<PortfolioOverview>>> Handle(GetPortfoliosQuery request, CancellationToken cancellationToken)
        {
            if (request.CreatedFromDate == null)
                request.CreatedFromDate = DateTime.UtcNow.AddYears(-10);
            
            var query = GetQueryRequest(request);
            var response = await _db.QueryAsync(query);
            var orderEntities = response.Items
                .Select(i => i.ToEntity<PortfolioEntity>())
                .ToList();

            var portfolioOverviews = orderEntities.Select(_mapper.Map<PortfolioOverview>);

            return BusinessResponse.Ok<IEnumerable<PortfolioOverview>>(portfolioOverviews);
        }

        /***
        * Query parameters
        ***/
        private const string V_CREATED_FROM_DATE = ":v_created_from_date";
        private const string V_NAME_QUERY = ":v_name_query";
        private const string V_REQUESTING_USER_ID = ":v_requesting_user_id";

        private QueryRequest GetQueryRequest(GetPortfoliosQuery request)
        {            
            var query = new QueryRequest
            {
                TableName = IAmazonDynamoDBExtensions.GetDynamoDBTableName<PortfolioEntity>(),
                Select = "ALL_ATTRIBUTES",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {V_CREATED_FROM_DATE, ((DateTime) request.CreatedFromDate).GetAttributeValue()},
                    {V_REQUESTING_USER_ID, new AttributeValue { S = request.RequestingUserId }}
                }
            };

            SetQueryConditions(query, request);

            return query;
        }

        private void SetQueryConditions(QueryRequest query, GetPortfoliosQuery request)
        {
            List<string> filterConditions = new List<string>(),
                keyConditions = new List<string>();
            
            if (request.NameQuery != null)
            {
                filterConditions.Add($"contains({nameof(PortfolioEntity.PortfolioName)}, {V_NAME_QUERY})");
                query.ExpressionAttributeValues.Add(
                    V_NAME_QUERY,
                    request.NameQuery.GetAttributeValue()
                );
            }

            keyConditions.Add($"({nameof(PortfolioEntity.CreatedAt)} >= {V_CREATED_FROM_DATE})");
            keyConditions.Add($"({nameof(OrderEntity.OwnerId)} = {V_REQUESTING_USER_ID})");

            query.KeyConditionExpression = string.Join(" AND ", keyConditions);

            if (filterConditions.Count > 0)
                query.FilterExpression = string.Join(" AND ", filterConditions);
        }
    }
}