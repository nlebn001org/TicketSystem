using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.RepositoryServices
{
    public interface IDeletableRepository<T>
    {
        void DeleteAsync(int id);
    }
}
