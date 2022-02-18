using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystem.Web.LogServices
{
    public class ConsoleLogService : ILogService
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
