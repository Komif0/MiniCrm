using Newtonsoft.Json;
using System.Security.Principal;
using static Program.Program;

public record Client(int Id, string Name, string Email, DateTime CreatedAt) : IEntity;
public record Order(int Id, int ClientId, string? Description, decimal Amount, DateOnly DueDate) : IEntity;

public class JsonFileStorage<T> : IStorage<T>
{
    private readonly string _filePath;

    public JsonFileStorage(string filePath)
    {
        _filePath = filePath;
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }
        var json = File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
    }

    public async Task SaveAsync(List<T> items)
    {
        var json = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);
        await File.WriteAllTextAsync(_filePath, json);
    }
}

public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity
{
    protected List<T> _items;
    IStorage<T> _storage;
    protected BaseRepository(IStorage<T> storage)
    {
        _storage = storage;
        _items = _storage.Load(); // Делегируем загрузку
    }

    public async Task SaveAsync()
    {
        await _storage.SaveAsync(_items); // Делегируем сохранение
    }
    public void Add(T entity) => _items.Add(entity);
    public IEnumerable<T> GetAll() => _items;
    public abstract T GetById(int id);

}

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(IStorage<Client> storage) : base(storage) { }
    public override Client GetById(int id)
    {
        return _items.FirstOrDefault(c => c.Id == id);
    }

    public int GetNextId()
    {
        if (_items.Count != 0)
        {
            var lastCl = _items.Cast<Client>().Last();
            return lastCl.Id + 1;
        }
        else { return 1; }
    }

}

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(IStorage<Order> storage) : base(storage) { }
    public override Order GetById(int id)
    {
        return _items.FirstOrDefault(o => o.Id == id);
    }

    public int GetNextId()
    {
        if (_items.Count != 0)
        {
            var lastCl = _items.Cast<Client>().Last();
            return lastCl.Id + 1;
        }
        else { return 1; }
    }
}