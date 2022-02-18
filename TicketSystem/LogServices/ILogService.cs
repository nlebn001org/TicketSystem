using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.LogServices
{
    public interface ILogService
    {
        void LogMessage(string message);
    }
}
