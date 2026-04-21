using Microsoft.EntityFrameworkCore.Storage;
using SocialNetwork.Data;

namespace SocialNetwork.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        return _dbContext.Database.BeginTransactionAsync(ct);
    }
}
