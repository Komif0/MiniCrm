using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace MiniCrm
{
    public class Program
    {

        public static void TestFunc() { }

        public static async Task Main(string[] args)
        {

            var crmService = CrmService.Instance;
            var notifier = new Notifier();
            var ui = new ConsoleUI(crmService, crmService, crmService);

            crmService.ClientAdded += notifier.OnClientAdded;

            ui.Run();

            //Добавлямс коммит
        }

    }
}