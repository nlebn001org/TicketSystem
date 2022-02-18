using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.RepositoryServices
{
    public interface IReadableRepository<T>
    {
        Task<IEnumerable<T>> getAllAsync();
        Task<T> GetByIdAsync(int id);
    }
}
