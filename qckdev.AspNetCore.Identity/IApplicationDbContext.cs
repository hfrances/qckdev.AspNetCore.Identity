using System;
using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity
{
    public interface IApplicationDbContext : IDisposable
    {

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

}
