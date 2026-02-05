public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(int id);
    void Add(T entity);
    Task SaveAsync();
    int GetNextId() { return 0; }
}

public interface IClientRepository : IRepository<Client>
{
    // Пока здесь пусто, но в будущем может появиться метод, например, FindByName(string name)
}

public interface IOrderRepository : IRepository<Order>
{
    // Здесь может появиться, например, IEnumerable<Order> GetByClientId(int clientId)
}

public interface IStorage<T>
{
    Task SaveAsync(List<T> items);
    List<T> Load();
}
public interface IEntity { int Id { get; } }

public interface IClientSearchStrategy
{
    bool IsMatch(Client client);
}