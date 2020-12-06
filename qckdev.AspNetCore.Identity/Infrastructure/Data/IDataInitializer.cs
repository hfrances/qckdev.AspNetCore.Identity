using System.Threading;
using System.Threading.Tasks;

namespace qckdev.AspNetCore.Identity.Infrastructure.Data
{
    public interface IDataInitializer
    {

        Task InitializeAsync(CancellationToken cancellationToken);

    }
}
