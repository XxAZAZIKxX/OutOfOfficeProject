using Microsoft.EntityFrameworkCore.Storage;
using OutOfOffice.Server.Data;

namespace OutOfOffice.Server.Repositories.Implementation;

/// <summary>
/// Represents a unit of work for database operations. Uses <see cref="DataContext"/>
/// </summary>
public class DbUnitOfWork(DataContext dataContext)
{
    /// <summary>
    /// Saves all changes made in this unit of work to the underlying database.
    /// </summary>
    public void SaveChanges() => dataContext.SaveChanges();

    /// <summary>
    /// Begins a database transaction synchronously.
    /// </summary>
    /// <returns>The transaction instance.</returns>
    public IDbContextTransaction BeginTransaction() => dataContext.Database.BeginTransaction();

    /// <summary>
    /// Commits the database transaction synchronously.
    /// </summary>
    public void CommitTransaction() => dataContext.Database.CommitTransaction();

    /// <summary>
    /// Rolls back the database transaction synchronously.
    /// </summary>
    public void RollbackChanges() => dataContext.Database.RollbackTransaction();

    /// <summary>
    /// Asynchronously saves all changes made in this unit of work to the underlying database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SaveChangesAsync() => dataContext.SaveChangesAsync();

    /// <summary>
    /// Asynchronously begins a database transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result is the transaction instance.</returns>
    public Task<IDbContextTransaction> BeginTransactionAsync() => dataContext.Database.BeginTransactionAsync();

    /// <summary>
    /// Asynchronously commits the database transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CommitTransactionAsync() => dataContext.Database.CommitTransactionAsync();

    /// <summary>
    /// Asynchronously rolls back the database transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RollbackChangesAsync() => dataContext.Database.RollbackTransactionAsync();
}