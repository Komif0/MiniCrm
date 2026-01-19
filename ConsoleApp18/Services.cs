using static Program.Program;

namespace Services
{
    public sealed class CrmService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IOrderRepository _orderRepository;

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

        public void AddClient(Client client)
        {
            _clientRepository.Add(client);
            _clientRepository.SaveAsync().Wait(); // .Wait() для простоты в консольном приложении
        }

        public IEnumerable<Client> GetAllClients() => _clientRepository.GetAll();
    }

}