using Microsoft.VisualBasic;
using Services;
using System;
using System.IO;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Program
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var crmService = CrmService.Instance;
            var notifier = new Notifier();
            var ui = new ConsoleUI(crmService);

            crmService.ClientAdded += notifier.OnClientAdded;

            ui.Run();
        }

    }
}