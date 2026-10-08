namespace KPI_HUB.Models.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public decimal Salary { get; set; }
    public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
}
