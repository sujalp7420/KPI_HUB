using KPI_HUB.Models.ViewModels;

namespace KPI_HUB.Services.Interfaces;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseListViewModel>> GetExpensesAsync();
    Task<EditExpenseViewModel?> GetExpenseForEditAsync(int id);
    Task AddExpenseAsync(CreateExpenseViewModel model);
    Task UpdateExpenseAsync(EditExpenseViewModel model);
    Task DeleteExpenseAsync(int id);
}
