using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.Web.LogServices;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.RepositoryServices
{
    public class UserRepository : EntityRepository<User>
    {
        public UserRepository(Func<SystemDbContext> contextFactory, ILogService log)
               : base(contextFactory, log)
        {

        }
    }
}
