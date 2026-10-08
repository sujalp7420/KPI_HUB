using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<EmployeeListViewModel>> GetEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return employees.Select(e => new EmployeeListViewModel
        {
            Id = e.Id,
            Name = e.Name,
            Email = e.Email,
            Department = e.Department,
            Position = e.Position,
            Salary = e.Salary,
            Status = e.Status
        });
    }

    public async Task<EditEmployeeViewModel?> GetEmployeeForEditAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return null;

        return new EditEmployeeViewModel
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Phone = employee.Phone,
            Department = employee.Department,
            Position = employee.Position,
            Salary = employee.Salary,
            JoiningDate = employee.JoiningDate,
            Status = employee.Status
        };
    }

    public async Task AddEmployeeAsync(CreateEmployeeViewModel model)
    {
        var employee = new Employee
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Department = model.Department,
            Position = model.Position,
            Salary = model.Salary,
            JoiningDate = model.JoiningDate,
            Status = model.Status
        };

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveAsync();
    }

    public async Task UpdateEmployeeAsync(EditEmployeeViewModel model)
    {
        var employee = await _employeeRepository.GetByIdAsync(model.Id);
        if (employee == null) return;

        employee.Name = model.Name;
        employee.Email = model.Email;
        employee.Phone = model.Phone;
        employee.Department = model.Department;
        employee.Position = model.Position;
        employee.Salary = model.Salary;
        employee.JoiningDate = model.JoiningDate;
        employee.Status = model.Status;

        _employeeRepository.Update(employee);
        await _employeeRepository.SaveAsync();
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return;

        _employeeRepository.Delete(employee);
        await _employeeRepository.SaveAsync();
    }
}
