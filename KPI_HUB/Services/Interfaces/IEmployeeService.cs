using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeListViewModel>> GetEmployeesAsync();
    Task<EditEmployeeViewModel?> GetEmployeeForEditAsync(int id);
    Task AddEmployeeAsync(CreateEmployeeViewModel model);
    Task UpdateEmployeeAsync(EditEmployeeViewModel model);
    Task DeleteEmployeeAsync(int id);
}
