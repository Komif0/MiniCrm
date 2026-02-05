using static Program.Program;

namespace Services
{
    public class SearchClientsByEmailStrategy : IClientSearchStrategy
    {
        private readonly string _emailDomain;

        public SearchClientsByEmailStrategy(string emailDomain)
        {
            _emailDomain = emailDomain.ToLower();
        }

        public bool IsMatch(Client client)
        {
            return client.Email.ToLower().EndsWith(_emailDomain);
        }
    }

    public class SearchClientsByNameStrategy : IClientSearchStrategy
    {
        private readonly string _name;

        public SearchClientsByNameStrategy(string name)
        {
            _name = name.ToLower();
        }

        public bool IsMatch(Client client)
        {
            return client.Name.ToLower().Contains(_name);
        }
    }


    public class Notifier
    {
        public void OnClientAdded(Client client)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Уведомление]: Добавлен новый клиент '{client.Name}' с Email: {client.Email}");
            Console.ResetColor();
        }
    }


    public sealed class CrmService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IOrderRepository _orderRepository;

        public event Action<Client> ClientAdded;



        private static readonly Lazy<CrmService> lazy = new Lazy<CrmService>(() =>
        {
            // 1. Создаем "специалистов" по хранению
            var clientStorage = new JsonFileStorage<Client>("clients.json");
            var orderStorage = new JsonFileStorage<Order>("orders.json");

            // 2. Создаем репозитории, передавая им зависимости хранения
            var clientRepo = new ClientRepository(clientStorage);
            var orderRepo = new OrderRepository(orderStorage);

            // 3. Создаем сервис, передавая ему репозитории
            return new CrmService(clientRepo, orderRepo);
        });

        public static CrmService Instance => lazy.Value;

        private CrmService(IClientRepository clientRepository, IOrderRepository orderRepository)
        {
            _clientRepository = clientRepository;
            _orderRepository = orderRepository;
        }

        public Client AddClient(string name, string email)
        {
            var client = new Client(_clientRepository.GetNextId(), name, email, DateTime.Now);
            _clientRepository.Add(client);
            _clientRepository.SaveAsync();

            // 2. Генерируем (вызываем) событие, уведомляя всех подписчиков.
            // Передаем в качестве аргумента только что созданного клиента.
            ClientAdded?.Invoke(client);

            return client;
        }

        public IEnumerable<Client> GetAllClients() => _clientRepository.GetAll();

        public IEnumerable<Client> FindClients(IClientSearchStrategy strategy)
        {
            return _clientRepository.GetAll().Where(client => strategy.IsMatch(client));
        }

        public IEnumerable<Order> GetAllOrders() => _orderRepository.GetAll();
    }
}