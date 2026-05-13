using ZaroShop.Server.Interfaces;

namespace ZaroShop.Server.Repositories
{
    public abstract class InMemoryRepositoryBase<T> : IRepository<T> where T : class
    {
        // Using a List as our "Database"
        protected readonly List<T> _collection = new();

        public virtual T? GetById(int id)
        {
            // This assumes your entities have an 'Id' property. 
            // In a real generic scenario, you might use an 'IEntity' interface.
            var entity = _collection.FirstOrDefault(x =>
                (int)x.GetType().GetProperty("Id")?.GetValue(x)! == id);

            return entity;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _collection.AsQueryable();
        }

        public void Add(T entity)
        {
            _collection.Add(entity);
        }

        public virtual void Update(T entity)
        {
            var guid = (Guid)entity.GetType().GetProperty("Id")?.GetValue(entity)!;
            var existing = _collection.FirstOrDefault(x =>
                (Guid)x.GetType().GetProperty("Id")?.GetValue(x)! == guid);

            if (existing != null)
            {
                _collection.Remove(existing);
                _collection.Add(entity);
            }
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if(entity != null) _collection.Remove(entity);
        }
    }
}
