using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IAmazonDynamoDB _dbClient;

        public ValuesController(IAmazonDynamoDB dbClient)
        {
            _dbClient = dbClient;
        }

        [HttpGet]
        [Route("tables")]
        public async Task<IEnumerable<string>> ListTables()
        {
            var request = new ListTablesRequest
            {
                Limit = 100
            };

            var response = await _dbClient.ListTablesAsync(request);
            List<string> tables = response.TableNames;

            return tables;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
