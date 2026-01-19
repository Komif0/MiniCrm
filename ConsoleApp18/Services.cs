using static Program.Program;

namespace Services
{
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
            var clientRepo = new ClientRepository("clients.json");
            var orderRepo = new OrderRepository("orders.json");
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
    }

}