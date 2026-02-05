using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class BaseReportGenerator
{
    protected readonly CrmService _crmService;

    protected BaseReportGenerator(CrmService crmService)
    {
        _crmService = crmService;
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
    public ClientOrdersReport(CrmService crmService) : base(crmService) { }

    protected override void GenerateBody()
    {
        Console.WriteLine("\n--- Детальный отчет по заказам клиентов ---");
        var clients = _crmService.GetAllClients();
        var allOrders = _crmService.GetAllOrders();

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
    public ClientListReport(CrmService crmService) : base(crmService) { }

    protected override void GenerateBody()
    {
        Console.WriteLine("\n--- Список всех клиентов ---");
        var clients = _crmService.GetAllClients();
        foreach (var client in clients)
        {
            Console.WriteLine($"ID: {client.Id}, Имя: {client.Name}, Email: {client.Email}");
        }
    }
}
