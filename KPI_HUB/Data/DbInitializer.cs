using KPI_HUB.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KPI_HUB.Data;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.MigrateAsync();

        await SeedRolesAsync();
        await SeedUsersAsync();
        await SeedDataAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roles = ["Founder", "Manager", "Accountant", "Admin"];

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new { Email = "admin@kpihub.com", Password = "Admin123!", Role = "Admin", FullName = "System Admin" },
            new { Email = "founder@kpihub.com", Password = "Founder123!", Role = "Founder", FullName = "Founder" },
            new { Email = "manager@kpihub.com", Password = "Manager123!", Role = "Manager", FullName = "Operations Manager" },
            new { Email = "accountant@kpihub.com", Password = "Accountant123!", Role = "Accountant", FullName = "Finance Lead" }
        };

        foreach (var userData in users)
        {
            var existingUser = await _userManager.FindByEmailAsync(userData.Email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userData.Email,
                    Email = userData.Email,
                    FullName = userData.FullName,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, userData.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userData.Role);
                }
            }
        }
    }

    private async Task SeedDataAsync()
    {
        if (await _context.Customers.AnyAsync()) return;

        var customers = new[]
        {
            new Customer { Name = "Apex Retail", Email = "sales@apexretail.com", Phone = "5550101", Address = "New York", RegistrationDate = DateTime.UtcNow.AddMonths(-6), Status = "Active", TotalPurchases = 15000m },
            new Customer { Name = "BluePeak Labs", Email = "hello@bluepeaklabs.com", Phone = "5550102", Address = "Boston", RegistrationDate = DateTime.UtcNow.AddMonths(-3), Status = "Active", TotalPurchases = 9200m },
            new Customer { Name = "Nova Health", Email = "contact@novahealth.com", Phone = "5550103", Address = "Chicago", RegistrationDate = DateTime.UtcNow.AddMonths(-2), Status = "Active", TotalPurchases = 11800m },
            new Customer { Name = "Luma Studio", Email = "info@lumastudio.io", Phone = "5550104", Address = "Austin", RegistrationDate = DateTime.UtcNow.AddMonths(-1), Status = "Active", TotalPurchases = 8400m }
        };

        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

        var products = new[]
        {
            new Product { Name = "Starter SaaS", Description = "Core platform subscription", Category = "Software", Price = 399m, CostPrice = 170m, StockQuantity = 200, MinimumStock = 25, Status = "Active" },
            new Product { Name = "Analytics Pro", Description = "Advanced analytics package", Category = "Software", Price = 899m, CostPrice = 420m, StockQuantity = 120, MinimumStock = 15, Status = "Active" },
            new Product { Name = "Website Setup", Description = "Business website package", Category = "Service", Price = 1500m, CostPrice = 600m, StockQuantity = 50, MinimumStock = 10, Status = "Active" },
            new Product { Name = "Consulting Retainer", Description = "Monthly strategy advisory", Category = "Service", Price = 2200m, CostPrice = 900m, StockQuantity = 30, MinimumStock = 5, Status = "Active" }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var sales = new[]
        {
            new Sale { CustomerId = customers[0].Id, SaleDate = DateTime.UtcNow.AddDays(-30), TotalAmount = 3990m, PaymentMethod = "Card", Status = "Completed" },
            new Sale { CustomerId = customers[1].Id, SaleDate = DateTime.UtcNow.AddDays(-20), TotalAmount = 899m, PaymentMethod = "Bank Transfer", Status = "Completed" },
            new Sale { CustomerId = customers[2].Id, SaleDate = DateTime.UtcNow.AddDays(-12), TotalAmount = 1500m, PaymentMethod = "Card", Status = "Completed" },
            new Sale { CustomerId = customers[3].Id, SaleDate = DateTime.UtcNow.AddDays(-5), TotalAmount = 2200m, PaymentMethod = "Cash", Status = "Completed" }
        };

        _context.Sales.AddRange(sales);
        await _context.SaveChangesAsync();

        var saleItems = new[]
        {
            new SaleItem { SaleId = sales[0].Id, ProductId = products[0].Id, Quantity = 10, UnitPrice = 399m, TotalPrice = 3990m },
            new SaleItem { SaleId = sales[1].Id, ProductId = products[1].Id, Quantity = 1, UnitPrice = 899m, TotalPrice = 899m },
            new SaleItem { SaleId = sales[2].Id, ProductId = products[2].Id, Quantity = 1, UnitPrice = 1500m, TotalPrice = 1500m },
            new SaleItem { SaleId = sales[3].Id, ProductId = products[3].Id, Quantity = 1, UnitPrice = 2200m, TotalPrice = 2200m }
        };

        _context.SaleItems.AddRange(saleItems);

        var revenues = new[]
        {
            new Revenue { Date = DateTime.UtcNow.AddMonths(-2), Amount = 54000m, Source = "Subscription", Description = "Monthly recurring revenue", CustomerId = customers[0].Id, PaymentMethod = "Card", ReferenceNumber = "REV-1001" },
            new Revenue { Date = DateTime.UtcNow.AddMonths(-1), Amount = 46000m, Source = "Services", Description = "Consulting revenue", CustomerId = customers[1].Id, PaymentMethod = "Bank Transfer", ReferenceNumber = "REV-1002" },
            new Revenue { Date = DateTime.UtcNow, Amount = 52000m, Source = "Product Sales", Description = "Current month sales", CustomerId = customers[2].Id, PaymentMethod = "Card", ReferenceNumber = "REV-1003" }
        };

        _context.Revenues.AddRange(revenues);

        var expenses = new[]
        {
            new Expense { Date = DateTime.UtcNow.AddMonths(-2), Amount = 25000m, Category = "Salary", Description = "Team payroll", PaymentMethod = "Bank Transfer", Vendor = "PeopleOps", Notes = "Monthly payroll" },
            new Expense { Date = DateTime.UtcNow.AddMonths(-1), Amount = 14000m, Category = "Marketing", Description = "Ad campaign", PaymentMethod = "Card", Vendor = "Google Ads", Notes = "Lead generation" },
            new Expense { Date = DateTime.UtcNow, Amount = 28000m, Category = "Software", Description = "Tooling and subscriptions", PaymentMethod = "Card", Vendor = "Cloud Tools", Notes = "SaaS stack" }
        };

        _context.Expenses.AddRange(expenses);

        var campaigns = new[]
        {
            new MarketingCampaign { Name = "Spring Launch", StartDate = DateTime.UtcNow.AddDays(-45), EndDate = DateTime.UtcNow.AddDays(-15), Platform = "Google Ads", Budget = 7000m, ActualCost = 6500m, Leads = 180, CustomersAcquired = 35, RevenueGenerated = 21000m, Status = "Completed" },
            new MarketingCampaign { Name = "Growth Sprint", StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(-5), Platform = "LinkedIn", Budget = 5000m, ActualCost = 4800m, Leads = 120, CustomersAcquired = 24, RevenueGenerated = 16500m, Status = "Completed" },
            new MarketingCampaign { Name = "Brand Push", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(20), Platform = "Meta", Budget = 6000m, ActualCost = 4200m, Leads = 150, CustomersAcquired = 18, RevenueGenerated = 11500m, Status = "Active" }
        };

        _context.MarketingCampaigns.AddRange(campaigns);

        var employees = new[]
        {
            new Employee { Name = "Sarah Nguyen", Email = "sarah@kpihub.com", Phone = "5550201", Department = "Engineering", Position = "CTO", Salary = 120000m, JoiningDate = DateTime.UtcNow.AddYears(-2), Status = "Active" },
            new Employee { Name = "Daniel Brooks", Email = "daniel@kpihub.com", Phone = "5550202", Department = "Sales", Position = "Sales Lead", Salary = 90000m, JoiningDate = DateTime.UtcNow.AddYears(-1), Status = "Active" },
            new Employee { Name = "Maya Singh", Email = "maya@kpihub.com", Phone = "5550203", Department = "Finance", Position = "Accountant", Salary = 85000m, JoiningDate = DateTime.UtcNow.AddMonths(-8), Status = "Active" }
        };

        _context.Employees.AddRange(employees);

        await _context.SaveChangesAsync();
    }
}
