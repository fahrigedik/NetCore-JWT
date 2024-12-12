using AuthServer.Core.UnitOfWork;

namespace AuthServer.Data.UnitOfWork;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public async Task RollbackAsync()
    {
        await context.Database.RollbackTransactionAsync();
    }

    public void Rollback()
    {
        context.Database.RollbackTransaction();
    }
}

