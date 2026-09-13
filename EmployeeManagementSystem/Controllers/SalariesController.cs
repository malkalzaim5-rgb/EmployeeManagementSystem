using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Data;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class SalariesController : Controller
    {
        private readonly AppDbContext _context;

        public SalariesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Salaries
        public async Task<IActionResult> Index()
        {
            var salaries = _context.Salaries.Include(s => s.Employee);
            return View(await salaries.ToListAsync());
        }

        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null) return NotFound();
            return View(salary);
        }

        // GET: Salaries/Create
        public IActionResult Create()
        {
            var employeesWithoutSalary = _context.Employees
                .Where(e => !_context.Salaries.Any(s => s.EmployeeId == e.Id))
                .ToList();

            ViewData["EmployeeId"] = new SelectList(employeesWithoutSalary, "Id", "Name");
            return View();
        }

        // POST: Salaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,BasicSalary,Bonus")] Salary salary)
        {
            ModelState.Remove("Employee");
            if (ModelState.IsValid)
            {
                _context.Add(salary);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Salary record created successfully!";
                return RedirectToAction(nameof(Index));
            }

            var employeesWithoutSalary = _context.Employees
                .Where(e => !_context.Salaries.Any(s => s.EmployeeId == e.Id))
                .ToList();
            ViewData["EmployeeId"] = new SelectList(employeesWithoutSalary, "Id", "Name", salary.EmployeeId);
            return View(salary);
        }

        // GET: Salaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null) return NotFound();

            var availableEmployees = _context.Employees
                .Where(e => e.Id == salary.EmployeeId || !_context.Salaries.Any(s => s.EmployeeId == e.Id))
                .ToList();

            ViewData["EmployeeId"] = new SelectList(availableEmployees, "Id", "Name", salary.EmployeeId);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,BasicSalary,Bonus")] Salary salary)
        {
            if (id != salary.Id) return NotFound();
            ModelState.Remove("Employee");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salary);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Salary record updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryExists(salary.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var availableEmployees = _context.Employees
                .Where(e => e.Id == salary.EmployeeId || !_context.Salaries.Any(s => s.EmployeeId == e.Id))
                .ToList();
            ViewData["EmployeeId"] = new SelectList(availableEmployees, "Id", "Name", salary.EmployeeId);
            return View(salary);
        }

        // GET: Salaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null) return NotFound();
            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary != null)
            {
                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Salary record deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryExists(int id)
        {
            return _context.Salaries.Any(e => e.Id == id);
        }
    }
}