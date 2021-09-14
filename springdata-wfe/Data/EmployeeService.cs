using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using springdata_common.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace springdata_wfe.Data
{
    public class EmployeeService
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly IConfiguration _configuration;
        static readonly HttpClient httpClient = new HttpClient();

        public EmployeeService(
            ILogger<EmployeeService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Employee[]> GetEmployeesAsync()
        {
            var apiAppHostUrl = _configuration["apiApp_HostUrl"];
            var result = await httpClient.GetAsync($"{apiAppHostUrl}/api/employee");

            string resultsJson = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Employee[]>(resultsJson);
        }

        public async Task<Employee> AddEmployeesAsync(Employee employee)
        {
            employee.Id = Guid.NewGuid().ToString();
            var apiAppHostUrl = _configuration["apiApp_HostUrl"];
            var json = JsonConvert.SerializeObject(employee);
            var stringContent = new StringContent(json, Encoding.UTF32, "application/json");
            var result = await httpClient.PostAsync($"{apiAppHostUrl}/api/employee", stringContent);

            if (result.IsSuccessStatusCode)
            {
                string resultJson = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Employee>(resultJson);
            }

            throw new HttpListenerException(1, result.StatusCode.ToString());
        }
    }
}
