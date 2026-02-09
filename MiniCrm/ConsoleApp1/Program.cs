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

        public static void Main(string[] args)
        {

            var crmService = CrmService.Instance;
            var notifier = new Notifier();
            var ui = new ConsoleUI(crmService, crmService, crmService);

            crmService.ClientAdded += notifier.OnClientAdded;

            ReportGeneratorFactory clientReportFactory = new ClientListReportFactory();
            ReportGeneratorFactory ordersReportFactory = new ClientOrdersReportFactory();

            BaseReportGenerator clientReport = clientReportFactory.CreateGenerator(crmService, crmService);
            BaseReportGenerator ordersReport = ordersReportFactory.CreateGenerator(crmService, crmService);

            ui.Run();
        }

    }
}