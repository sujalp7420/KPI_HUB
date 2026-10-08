using KPI_HUB.Data;
using KPI_HUB.Models.Entities;
using KPI_HUB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Repositories.Implementations;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
    {
        return await _context.Employees
            .Where(e => e.Department == department)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalMonthlySalaryExpenseAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Employees
            .Where(e => e.Status == "Active")
            .SumAsync(e => e.Salary);
    }
}
