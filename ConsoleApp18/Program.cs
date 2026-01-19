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
            // 1. Получаем экземпляр нашего сервиса-одиночки
            var crmService = CrmService.Instance;

            // 2. Создаем экземпляр подписчика
            var notifier = new Notifier();

            // 3. Подписываем его метод OnClientAdded на событие ClientAdded
            crmService.ClientAdded += notifier.OnClientAdded;

            // 4. Имитируем работу приложения для проверки
            Console.WriteLine("--- Система CRM запущена ---");
            Console.WriteLine("Добавляем клиента...");
            crmService.AddClient("Иван Иванов", "ivan@example.com");

            Console.ReadLine();

            // 3. Подписываем его метод OnClientAdded на событие ClientAdded
            crmService.ClientAdded -= notifier.OnClientAdded;

            Console.WriteLine("--- Система CRM запущена ---");
            Console.WriteLine("Добавляем клиента...");
            crmService.AddClient("Иван Иванов", "ivan@example.com");
        }

    }
}