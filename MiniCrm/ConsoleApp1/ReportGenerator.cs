using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCrm
{
    public abstract class ReportGeneratorFactory
    {
        // Фабричный метод
        public abstract BaseReportGenerator CreateGenerator(IClientReader clientReader, IOrderReader orderReader);
    }

    public class ClientListReportFactory : ReportGeneratorFactory
    {
        public override BaseReportGenerator CreateGenerator(IClientReader clientReader, IOrderReader orderReader)
        {
            return new ClientListReport(clientReader, orderReader);
        }
    }

    public class ClientOrdersReportFactory : ReportGeneratorFactory
    {
        public override BaseReportGenerator CreateGenerator(IClientReader clientReader, IOrderReader orderReader)
        {
            return new ClientOrdersReport(clientReader, orderReader);
        }
    }


    public abstract class BaseReportGenerator
    {
        protected readonly IClientReader _clientReader;
        protected readonly IOrderReader _orderReader;

        protected BaseReportGenerator(IClientReader clientReader, IOrderReader orderReader)
        {
            _clientReader = clientReader;
            _orderReader = orderReader;
        }
        public void Generate()
        {
            GenerateHeader();
            GenerateBody();
            GenerateFooter();
        }
        protected virtual void GenerateHeader()
        {
            Console.WriteLine("===================================");
            Console.WriteLine("        ОТЧЕТ ПО СИСТЕМЕ CRM       ");
            Console.WriteLine("===================================");
        }
        protected virtual void GenerateFooter()
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Отчет сгенерирован: {DateTime.Now}");
            Console.WriteLine("===================================");
        }
        protected abstract void GenerateBody();
    }
    public class ClientOrdersReport : BaseReportGenerator
    {
        public ClientOrdersReport(IClientReader clientReader, IOrderReader orderReader) : base(clientReader, orderReader) { }

        protected override void GenerateBody()
        {
            Console.WriteLine("\n--- Детальный отчет по заказам клиентов ---");
            var clients = _clientReader.GetAllClients();
            var allOrders = _orderReader.GetAllOrders();

            foreach (var client in clients)
            {
                Console.WriteLine($"\nКлиент: {client.Name} (ID: {client.Id})");
                var clientOrders = allOrders.Where(o => o.ClientId == client.Id);
                if (clientOrders.Any())
                {
                    foreach (var order in clientOrders)
                    {
                        Console.WriteLine($"  - Заказ #{order.Id}: {order.Description} на сумму {order.Amount:C}");
                    }
                }
                else
                {
                    Console.WriteLine("  - Заказов нет.");
                }
            }
        }
    }
    public class ClientListReport : BaseReportGenerator
    {
        public ClientListReport(IClientReader clientReader, IOrderReader orderReader) : base(clientReader, orderReader) { }

        protected override void GenerateBody()
        {
            Console.WriteLine("\n--- Список всех клиентов ---");
            var clients = _clientReader.GetAllClients();
            foreach (var client in clients)
            {
                Console.WriteLine($"ID: {client.Id}, Имя: {client.Name}, Email: {client.Email}");
            }
        }
    }

}