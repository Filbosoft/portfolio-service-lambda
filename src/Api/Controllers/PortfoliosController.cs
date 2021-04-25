using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Api.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfoliosController(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        [HttpPost]
        public async Task<Portfolio> CreatePortfolio([FromBody] Portfolio portfolio)
        {
            var newPortfolio = await _portfolioRepository.CreatePortfolio(portfolio);
            return newPortfolio;
        }

        [HttpGet]
        public async Task<IEnumerable<Portfolio>> Get()
        {
            var portfolios = await _portfolioRepository.GetPortfolios();
            return portfolios;
        }

        [HttpGet("{id}")]
        public async Task<Portfolio> Get([FromRoute] string id)
        {
            var portfolio = await _portfolioRepository.GetPortfolio(id);
            return portfolio;
        }


        [HttpPut("{id}")]
        public async Task<Portfolio> Put([FromRoute] string id, [FromBody] Portfolio portfolio)
        {
            var updatedPortfolio = await _portfolioRepository.UpdatePortfolio(portfolio);
            return updatedPortfolio;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete([FromRoute] string id)
        {
            var success = await _portfolioRepository.DeletePortfolio(id);
            return success;
        }
    }
}
