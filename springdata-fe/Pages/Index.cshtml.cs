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

        [BindProperty]
        public Employee Employee { get; set; }

        public Employee[] Employees { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task OnGet()
        {
            Employee = new Employee();
            Employee.Id = Guid.NewGuid().ToString();

            var employeeService = new EmployeeService(_configuration);
            this.Employees = await employeeService.GetEmployeesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var employeeService = new EmployeeService(_configuration);

            if (!ModelState.IsValid)
            {
                this.Employees = await employeeService.GetEmployeesAsync();
                return Page();
            }

            await employeeService.AddEmployeeAsync(Employee);
            
            return RedirectToPage("/Index");
        }
    }
}
