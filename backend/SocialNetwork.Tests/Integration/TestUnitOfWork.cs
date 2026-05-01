using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using SocialNetwork.Repository;

namespace SocialNetwork.Tests.Integration;

public sealed class TestUnitOfWork : IUnitOfWork
{
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IDbContextTransaction>(new NoOpDbContextTransaction());
    }

    private sealed class NoOpDbContextTransaction : IDbContextTransaction
    {
        public Guid TransactionId { get; } = Guid.NewGuid();

        public void Commit() { }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Rollback() { }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void CreateSavepoint(string name) { }

        public Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void RollbackToSavepoint(string name) { }

        public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void ReleaseSavepoint(string name) { }

        public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public DbTransaction? GetDbTransaction()
        {
            return null;
        }

        public void Dispose() { }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
