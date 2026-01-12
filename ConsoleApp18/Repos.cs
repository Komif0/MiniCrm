using Newtonsoft.Json;
using static Program.Program;

public abstract class BaseRepository<T>
{
    protected List<T> _items;
    protected readonly string _filePath;
    protected int _nextId = 1;

    protected BaseRepository(string filePath)
    {
        _filePath = filePath;
        _items = new List<T>();
        Load();
    }
    public virtual async Task SaveAsync()
    {
        string json = JsonConvert.SerializeObject(_items,
            Newtonsoft.Json.Formatting.Indented);

        await File.WriteAllTextAsync(_filePath, json);
    }

    public List<T> GetAll() => _items;
      
    private void Load()
    {
        if (!File.Exists(_filePath)) return;
        string json = File.ReadAllText(_filePath);
        // ћы пока не можем быть уверены, что T - это ссылочный тип, поэтому проверка на null важна
        _items = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
    }

}

public class ClientRepository : BaseRepository<Client>
{
    public ClientRepository(string filePath) : base (filePath)
    {
        if (_items.Any())
        {
            _nextId = _items.Cast<Client>().Max(c => c.id) + 1;
        }
    }

    public Client Add(string name, string email)
    {
        var client = new Client(name, email, _nextId++, DateTime.Now);
        _items.Add(client);
        return client;
    }

    public Client? GetById(int id)
    {
        return _items.Cast<Client>().FirstOrDefault(c => c.id == id);
    }
}


public class OrderRepository : BaseRepository<Order> 
{
    public OrderRepository(string filePath) : base(filePath)
    {
        if (_items.Any())
        {
            _nextId = _items.Cast<Order>().Max(o => o.id) + 1;
        }
    }


    public Order Add(int Id, int ClientId, string? Description, decimal Amount, DateOnly DueDate)
    {
        var order = new Order(Id, ClientId, Description, Amount, DueDate);
        _items.Add(order);
        return order;
    }

    
    public List<Order> GetOrdersByClientId(int clientId) 
    {
        return _items.Cast<Order>().Where(o => o.ClientId == clientId).ToList();
    } 
}