using static Program.Program;

namespace Services 
{
    public sealed class CrmService 
    {
        private static readonly CrmService _instance = new CrmService();
        private readonly ClientRepository _clientRepository;
        private readonly OrderRepository _orderRepository;


        private CrmService()
        {
            Console.WriteLine("Экземпляр CrmService создан.");
            _clientRepository = new ClientRepository("clients.json");
            _orderRepository = new OrderRepository("orders.json");
        }

        public static CrmService Instance => _instance;

        public async Task AddClientAsync(string name, string email) 
        {
            _clientRepository.Add(name, email);
            await _clientRepository.SaveAsync();
            Console.WriteLine($"Клиент '{name}' успешно добавлен.");
        }

        public List<Order> GetClientOrders(int clientId)
        {
            return _orderRepository.GetOrdersByClientId(clientId);
        }

        public List<Client> GetAllClients() 
        {
            return _clientRepository.GetAll(); 
        }

    }

}