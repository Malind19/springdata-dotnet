using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using springdata_api.Models;
using springdata_common.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace springdata_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IConfiguration _configuration;
        static readonly HttpClient httpClient = new HttpClient();

        public EmployeeController(
            ILogger<EmployeeController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Employee> Post(Employee employee)
        {
            var cosmosDbEndpoint = _configuration["cosmosDB_Endpoint"];
            var databaseName = _configuration["cosmosDB_Name"];
            var containerName = _configuration["cosmosDB_Containers_Employees"];
            var accessKeys = await GetDbKeys();
            CosmosClient client = new CosmosClient(cosmosDbEndpoint, accessKeys.primaryMasterKey);

            var database = client.GetDatabase(databaseName);
            var container = database.GetContainer(containerName);

            return await container.CreateItemAsync<Employee>(employee);
        }

        public async Task<List<Employee>> Get()
        {
            var cosmosDbEndpoint = _configuration["cosmosDB_Endpoint"];
            var databaseName = _configuration["cosmosDB_Name"];
            var containerName = _configuration["cosmosDB_Containers_Employees"];
            var accessKeys = await GetDbKeys();
            CosmosClient client = new CosmosClient(cosmosDbEndpoint, accessKeys.primaryMasterKey);

            var database = client.GetDatabase(databaseName);
            var container = database.GetContainer(containerName);

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c");

            FeedIterator<Employee> queryResultSetIterator
                = container.GetItemQueryIterator<Employee>(queryDefinition);
            var employees = new List<Employee>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Employee> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var result in currentResultSet)
                    employees.Add(result);
            }

            return employees;
        }

        internal async Task<DatabaseAccountListKeysResult> GetDbKeys()
        {
            var subscriptionId = _configuration["subscription_Id"];
            var resourceGroupName = _configuration["resourceGroup_Name"];
            var accountName = _configuration["cosmosDB_AccountName"];
            
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            _logger.LogDebug("Loading");
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com/");
            _logger.LogDebug(accessToken);

            string endpoint = $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.DocumentDB/databaseAccounts/{accountName}/listKeys?api-version=2019-12-12";


            // Add the access token to request headers.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Post to the endpoint to get the keys result.
            var result = await httpClient.PostAsync(endpoint, new StringContent(""));

            // Get the result back as a DatabaseAccountListKeysResult.
            string keysJson = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<DatabaseAccountListKeysResult>(keysJson);
        }
    }
}
