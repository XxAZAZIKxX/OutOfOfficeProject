using Microsoft.EntityFrameworkCore.Storage;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implemetation;

public class DbUnitOfWork(DataContext dataContext)
{
    public void SaveChanges() => dataContext.SaveChanges();
    public IDbContextTransaction BeginTransaction() => dataContext.Database.BeginTransaction();
    public void CommitTransaction() => dataContext.Database.CommitTransaction();
    public void RollbackChanges() => dataContext.Database.RollbackTransaction();

    public Task SaveChangesAsync() => dataContext.SaveChangesAsync();
    public Task<IDbContextTransaction> BeginTransactionAsync()=>dataContext.Database.BeginTransactionAsync();
    public Task CommitTransactionAsync() => dataContext.Database.CommitTransactionAsync();
    public Task RollbackChangesAsync() => dataContext.Database.RollbackTransactionAsync();
}