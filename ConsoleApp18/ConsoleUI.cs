using Services;


    public class ConsoleUI
    {
        private readonly CrmService _crmService;

        public ConsoleUI(CrmService crmService)
        {
            _crmService = crmService;
        }

        public void Run()
        {
            Console.WriteLine("--- Система CRM запущена ---");
            Console.WriteLine("Добавляем первого клиента...");
            _crmService.AddClient("Иван Иванов", "ivan@example.com");

            Console.WriteLine("\nДобавляем второго клиента...");
            _crmService.AddClient("Мария Петрова", "maria@example.com");

            // Обратите внимание: код подписки/отписки НЕ переносим,
            // так как это часть сборки, а не UI.
            // Но для демонстрации отписки нам нужен сам объект Notifier.
            // Давайте пока упростим и уберем логику отписки из UI.
            // Мы вернемся к этому, когда будем делать полноценное меню.

            Console.WriteLine("\nДобавляем третьего клиента...");
            _crmService.AddClient("Петр Сидоров", "petr@example.com");


        Console.WriteLine("\n--- Демонстрация паттерна Стратегия ---");

        // 1. Создаем стратегию для поиска по имени "Иван"
        var nameStrategy = new SearchClientsByNameStrategy("Иван");
        var foundByName = _crmService.FindClients(nameStrategy);
        Console.WriteLine("\nНайдены клиенты по имени 'Иван':");
        foreach (var client in foundByName)
        {
            Console.WriteLine(client);
        }

        // 2. Создаем стратегию для поиска по email-домену "@example.com"
        var emailStrategy = new SearchClientsByEmailStrategy("@example.com");
        var foundByEmail = _crmService.FindClients(emailStrategy);
        Console.WriteLine("\nНайдены клиенты с почтой на домене '@example.com':");
        foreach (var client in foundByEmail)
        {
            Console.WriteLine(client);
        }

        Console.WriteLine("\n\n--- Демонстрация паттерна Шаблонный метод ---");

        // 1. Создаем и запускаем простой отчет
        BaseReportGenerator clientReport = new ClientListReport(_crmService);
        Console.WriteLine("\n--- Генерация простого отчета по клиентам ---");
        clientReport.Generate();

        // 2. Создаем и запускаем сложный отчет
        BaseReportGenerator ordersReport = new ClientOrdersReport(_crmService);
        Console.WriteLine("\n\n--- Генерация детального отчета по заказам ---");
        ordersReport.Generate();


        Console.ReadLine();

    }
    }
