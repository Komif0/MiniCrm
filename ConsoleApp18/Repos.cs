using Newtonsoft.Json;
using static Program.Program;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(int id);
    void Add(T entity);
    Task SaveAsync();
}

public interface IClientRepository : IRepository<Client>
{
    // Пока здесь пусто, но в будущем может появиться метод, например, FindByName(string name)
}

public interface IOrderRepository : IRepository<Order>
{
    // Здесь может появиться, например, IEnumerable<Order> GetByClientId(int clientId)
}


public record Client(int Id, string Name, string Email, DateTime CreatedAt);
public record Order(int Id, int ClientId, string? Description, decimal Amount, DateOnly DueDate);
public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly string _filePath;
    protected List<T> _items;

    protected BaseRepository(string filePath)
    {
        _filePath = filePath;
        _items = new List<T>();
        Load();
    }

    private void Load()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _items = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }
    }

    public void Add(T entity) => _items.Add(entity);
    public IEnumerable<T> GetAll() => _items;
    public abstract T GetById(int id);
    public async Task SaveAsync()
    {
        var json = JsonConvert.SerializeObject(_items, Newtonsoft.Json.Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(string filePath) : base(filePath) { }
    public override Client GetById(int id)
    {
        return _items.FirstOrDefault(c => c.Id == id);
    }
}

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(string filePath) : base(filePath) { }
    public override Order GetById(int id)
    {
        return _items.FirstOrDefault(o => o.Id == id);
    }
}
