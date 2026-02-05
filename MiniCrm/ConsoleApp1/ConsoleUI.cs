namespace MiniCrm
{
    public class ConsoleUI
    {
        private readonly IClientReader _clientReader;
        private readonly IClientWriter _clientWriter;
        private readonly IOrderReader _orderReader;
        public ConsoleUI(IClientReader clientReader, IClientWriter clientWriter, IOrderReader orderReader)
        {
            _clientReader = clientReader;
            _clientWriter = clientWriter;
            _orderReader = orderReader;
        }

        public void Run()
        {
            Console.WriteLine("--- Система CRM запущена ---");
            Console.WriteLine("Добавляем первого клиента...");
            _clientWriter.AddClient("Иван Иванов", "ivan@example.com");

            Console.WriteLine("\nДобавляем второго клиента...");
            _clientWriter.AddClient("Мария Петрова", "maria@example.com");

            // Обратите внимание: код подписки/отписки НЕ переносим,
            // так как это часть сборки, а не UI.
            // Но для демонстрации отписки нам нужен сам объект Notifier.
            // Давайте пока упростим и уберем логику отписки из UI.
            // Мы вернемся к этому, когда будем делать полноценное меню.

            Console.WriteLine("\nДобавляем третьего клиента...");
            _clientWriter.AddClient("Петр Сидоров", "petr@example.com");


            Console.WriteLine("\n--- Демонстрация паттерна Стратегия ---");

            // 1. Создаем стратегию для поиска по имени "Иван"
            var nameStrategy = new SearchClientsByNameStrategy("Иван");
            var foundByName = _clientReader.FindClients(nameStrategy);
            Console.WriteLine("\nНайдены клиенты по имени 'Иван':");
            foreach (var client in foundByName)
            {
                Console.WriteLine(client);
            }

            // 2. Создаем стратегию для поиска по email-домену "@example.com"
            var emailStrategy = new SearchClientsByEmailStrategy("@example.com");
            var foundByEmail = _clientReader.FindClients(emailStrategy);
            Console.WriteLine("\nНайдены клиенты с почтой на домене '@example.com':");
            foreach (var client in foundByEmail)
            {
                Console.WriteLine(client);
            }

            Console.WriteLine("\n\n--- Демонстрация паттерна Шаблонный метод ---");

            // 1. Создаем и запускаем простой отчет
            BaseReportGenerator clientReport = new ClientListReport(_clientReader, _orderReader);
            Console.WriteLine("\n--- Генерация простого отчета по клиентам ---");
            clientReport.Generate();

            // 2. Создаем и запускаем сложный отчет
            BaseReportGenerator ordersReport = new ClientOrdersReport(_clientReader, _orderReader);
            Console.WriteLine("\n\n--- Генерация детального отчета по заказам ---");
            ordersReport.Generate();

            Console.ReadLine();
        }
    }
}
