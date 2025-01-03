﻿namespace AuthServer.Core.UnitOfWork;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
    void SaveChanges();
    Task RollbackAsync();
    void Rollback();
}

