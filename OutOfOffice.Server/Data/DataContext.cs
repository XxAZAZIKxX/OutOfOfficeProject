using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;

namespace OutOfOffice.Server.Data;
public sealed class DataContext : DbContext
{
    private static bool _isFirstCreation = true;

    public DbSet<Employee> Employees { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<AuthCredential> AuthCredentials { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        if (!_isFirstCreation) return;
        Database.EnsureCreated();
        _isFirstCreation = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthCredential>().HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId);
        base.OnModelCreating(modelBuilder);
    }
}