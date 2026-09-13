namespace EmployeeManagementSystem.Models
{
    public class Salary
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal Bonus { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}
