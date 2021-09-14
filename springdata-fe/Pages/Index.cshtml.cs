using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using springdata_common.Models;
using springdata_fe.Data;
using System;
using System.Threading.Tasks;

namespace springdata_fe.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public Employee[] Employees { get; set; }

        public Employee NewEmployee { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnGet()
        {
            var employeeService = new EmployeeService(_configuration);
            this.Employees = await employeeService.GetEmployeesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(NewEmployee.FirstName) && !string.IsNullOrEmpty(NewEmployee.LastName))
            {
                this.NewEmployee.Id = Guid.NewGuid().ToString();
                var employeeService = new EmployeeService(_configuration);
                var employee = await employeeService.AddEmployeeAsync(this.NewEmployee);
            }

            return RedirectToPage("./Index");
        }
    }
}
