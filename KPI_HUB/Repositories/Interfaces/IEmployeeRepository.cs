using KPI_HUB.Models.Entities;

namespace KPI_HUB.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);
    Task<decimal> GetTotalMonthlySalaryExpenseAsync();
}
