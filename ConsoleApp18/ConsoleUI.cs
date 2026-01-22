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

            Console.ReadLine();
        }
    }
