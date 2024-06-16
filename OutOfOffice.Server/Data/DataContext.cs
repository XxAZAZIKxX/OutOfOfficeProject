using Microsoft.EntityFrameworkCore;
using OutOfOffice.Core.Models;
using OutOfOffice.Server.Data.Models;

namespace OutOfOffice.Server.Data;
public sealed class DataContext : DbContext
{
    private static bool _isFirstCreation = true;

    public DbSet<Employee> Employees { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
    public DbSet<Project> Projects { get; set; }
    
    public DbSet<AuthCredential> AuthCredentials { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        if (!_isFirstCreation) return;
        Database.EnsureCreated();
        _isFirstCreation = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.PeoplePartner).WithMany().HasForeignKey(p => p.PeoplePartnerId);
        });
        modelBuilder.Entity<LeaveRequest>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Employee).WithMany().HasForeignKey(p => p.EmployeeId);
        });
        modelBuilder.Entity<ApprovalRequest>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.LeaveRequest).WithMany().HasForeignKey(p => p.LeaveRequestId);
            builder.HasOne(p => p.Approver).WithMany().HasForeignKey(p => p.ApproverId);
        });
        modelBuilder.Entity<Project>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.ProjectManager).WithMany().HasForeignKey(p => p.ProjectManagerId);
        });

        base.OnModelCreating(modelBuilder);
    }
}