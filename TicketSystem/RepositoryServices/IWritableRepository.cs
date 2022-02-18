using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.RepositoryServices
{
    public interface IWritableRepository<T>
    {
        void SaveAsync(T item);
    }
}
