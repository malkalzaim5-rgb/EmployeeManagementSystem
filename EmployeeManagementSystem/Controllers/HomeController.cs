using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;

namespace EmployeeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("HR"))
            {
                ViewBag.TotalDepartments = await _context.Departments.CountAsync();
                ViewBag.TotalEmployees = await _context.Employees.CountAsync();
                ViewBag.TotalSalaries = await _context.Salaries.SumAsync(s => (decimal?)(s.BasicSalary + s.Bonus)) ?? 0;

                // Data for Department Analytics Chart
                var deptData = await _context.Departments
                    .Select(d => new
                    {
                        DepartmentName = d.Name,
                        EmployeeCount = _context.Employees.Count(e => e.DepartmentId == d.Id)
                    })
                    .ToListAsync();

                ViewBag.DeptLabels = deptData.Select(d => d.DepartmentName).ToArray();
                ViewBag.DeptCounts = deptData.Select(d => d.EmployeeCount).ToArray();

                return View();
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userEmail = User.Identity.Name;

                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Email == userEmail);

                if (employee != null)
                {
                    ViewBag.Salary = await _context.Salaries
                        .FirstOrDefaultAsync(s => s.EmployeeId == employee.Id);
                }

                return View(employee);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}