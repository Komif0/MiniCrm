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
            Console.WriteLine("--- Демонстрация работы сервисного слоя ---");

            var newClient = new Client(3, "Ольга Иванова", "olga@test.com", DateTime.Now);
            CrmService.Instance.AddClient(newClient);

            Console.WriteLine("\nВсе клиенты в системе:");
            var allClients = CrmService.Instance.GetAllClients();
            foreach (var client in allClients)
            {
                Console.WriteLine(client);
            }

        }
    }
}