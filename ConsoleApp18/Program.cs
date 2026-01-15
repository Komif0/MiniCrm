using Microsoft.VisualBasic;
using System.Net;
using System.Reflection.PortableExecutable;
using System.IO;
using Services;
using System.Threading.Tasks;

namespace Program
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("============Программа запущена============");
            var crmServ = CrmService.Instance;

            Console.WriteLine("Добавил клиента");
            var clientsList = crmServ.GetAllClients();
            foreach (var client in clientsList) { Console.WriteLine($"Имя: {client.name} ID:{client.id}");  }

            await crmServ.AddClientAsync("Шайтан", "shaitan@gma.com");

            Console.WriteLine("Вывел клиента на экран");
            clientsList = crmServ.GetAllClients();
            foreach (var client in clientsList) { Console.WriteLine($"Имя: {client.name} ID:{client.id}"); }

        }
    }
}