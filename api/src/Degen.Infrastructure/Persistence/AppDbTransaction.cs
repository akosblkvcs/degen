using Degen.Application.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Degen.Infrastructure.Persistence;

internal sealed class AppDbTransaction(IDbContextTransaction transaction)
    : IAppDbTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        transaction.CommitAsync(cancellationToken);

    public ValueTask DisposeAsync() => transaction.DisposeAsync();
}
