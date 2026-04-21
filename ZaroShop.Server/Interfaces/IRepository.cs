namespace ZaroShop.Server.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    T? GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}