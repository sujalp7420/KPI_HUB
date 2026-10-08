using KPI_HUB.Models.Entities;
using KPI_HUB.Models.ViewModels;
using KPI_HUB.Repositories.Interfaces;
using KPI_HUB.Services.Interfaces;

namespace KPI_HUB.Services.Implementations;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<IEnumerable<ExpenseListViewModel>> GetExpensesAsync()
    {
        var expenses = await _expenseRepository.GetAllAsync();
        return expenses.OrderByDescending(e => e.Date).Select(e => new ExpenseListViewModel
        {
            Id = e.Id,
            Date = e.Date,
            Amount = e.Amount,
            Category = e.Category,
            Description = e.Description,
            PaymentMethod = e.PaymentMethod,
            Vendor = e.Vendor
        });
    }

    public async Task<EditExpenseViewModel?> GetExpenseForEditAsync(int id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null) return null;

        return new EditExpenseViewModel
        {
            Id = expense.Id,
            Date = expense.Date,
            Amount = expense.Amount,
            Category = expense.Category,
            Description = expense.Description,
            PaymentMethod = expense.PaymentMethod,
            Vendor = expense.Vendor,
            Notes = expense.Notes
        };
    }

    public async Task AddExpenseAsync(CreateExpenseViewModel model)
    {
        var expense = new Expense
        {
            Date = model.Date,
            Amount = model.Amount,
            Category = model.Category,
            Description = model.Description,
            PaymentMethod = model.PaymentMethod,
            Vendor = model.Vendor,
            Notes = model.Notes
        };

        await _expenseRepository.AddAsync(expense);
        await _expenseRepository.SaveAsync();
    }

    public async Task UpdateExpenseAsync(EditExpenseViewModel model)
    {
        var expense = await _expenseRepository.GetByIdAsync(model.Id);
        if (expense == null) return;

        expense.Date = model.Date;
        expense.Amount = model.Amount;
        expense.Category = model.Category;
        expense.Description = model.Description;
        expense.PaymentMethod = model.PaymentMethod;
        expense.Vendor = model.Vendor;
        expense.Notes = model.Notes;

        _expenseRepository.Update(expense);
        await _expenseRepository.SaveAsync();
    }

    public async Task DeleteExpenseAsync(int id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null) return;

        _expenseRepository.Delete(expense);
        await _expenseRepository.SaveAsync();
    }
}
