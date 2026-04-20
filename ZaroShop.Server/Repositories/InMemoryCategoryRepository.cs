using ZaroShop.Server.Interfaces;

namespace ZaroShop.Server.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _data = new();

    public IEnumerable<T> GetAll() => _data;

    public T? GetById(int id)
    {
        return _data.FirstOrDefault(x => (int?)x.GetType().GetProperty("Id")?.GetValue(x) == id);
    }

    public void Add(T entity)
    {
        _data.Add(entity);
    }

    public void Update(T entity)
    {
        var id = (int?)entity.GetType().GetProperty("Id")?.GetValue(entity);
        var existing = GetById(id ?? 0);
        if (existing != null)
        {
            _data.Remove(existing);
            _data.Add(entity);
        }
    }

    public void Delete(int id)
    {
        var entity = GetById(id);
        if (entity != null) _data.Remove(entity);
    }
}