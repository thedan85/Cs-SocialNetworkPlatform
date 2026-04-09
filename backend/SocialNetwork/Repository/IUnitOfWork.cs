using Microsoft.EntityFrameworkCore.Storage;

namespace SocialNetwork.Repository;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
