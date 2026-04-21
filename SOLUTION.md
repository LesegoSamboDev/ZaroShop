# SOLUTION.md — ZaroShop Design & Trade-offs

## Overview

ZaroShop is a full-stack e-commerce API built with **ASP.NET Core 10** and **Angular 21**. 
With **Bootstrap** as the styling component.

---

## Architecture

### Layered Structure

```
HTTP Request
    │
    ▼
Controller          ← Thin. Routing, input validation, HTTP concerns only.
    │
    ▼
Repository          ← Data access via Entity Framework Core. IQueryable + Include().
    │
    ▼
Utility / Engine    ← Pure in-memory logic (SearchEngine<T>). No EF dependency.
    │
    ▼
Cache Layer         ← Dictionary<string, List<T>> on the repository instance.
```

---

## Key Design Decision: `IQueryable<T>` on `GetAll()` Instead of Entity-Specific Interfaces

### The Problem with Entity-Specific Interfaces

A common pattern in repository implementations is to define a dedicated interface
per entity with `IEnumerable<T>` return types:

```csharp
// Approach NOT taken
public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    IEnumerable<Product> GetByCategory(int categoryId);
    // ...
}

public interface IOrderRepository
{
    IEnumerable<Order> GetAll();
    // ...
}
```

This approach has a compounding cost: every new entity requires a new interface,
a new concrete implementation, and new DI registrations. The interfaces also tend
to diverge over time as each entity accumulates its own query methods, making the
codebase harder to navigate and maintain.

### The Chosen Approach: `IQueryable<T>` on a Generic Repository

Instead, `GetAll()` returns `IQueryable<T>`, which defers query composition to the
caller:

```csharp
public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
}

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    public Repository(IEnumerable<T> items) => _context = context;
    public IQueryable<T> GetAll() => _context.Set<T>();
}
```

Any entity — `Product`, `Order`, `Customer` — is served by the same interface and
the same concrete class. Adding a new entity to the application requires zero new
repository code.

### Why `IQueryable<T>` and Not `IEnumerable<T>`

`IEnumerable<T>` materialises the data into memory immediately. Any filtering,
sorting, or joining applied after the call executes in-process on the full dataset:

```csharp
// IEnumerable — fetches ALL products first, then filters in memory
_repo.GetAll().Where(p => p.CategoryId == id)
```

`IQueryable<T>` keeps the query as an expression tree that EF Core translates to SQL.
The filter, join, and projection are pushed down to the database:

```csharp
// IQueryable — WHERE clause is sent to the database, not evaluated in C#
_repo.GetAll()
     .Include(p => p.Category)
     .Where(p => p.CategoryId == id)
     .ToList();
```

This means as the catalogue grows, the database does the heavy lifting rather than
the application server. A call like:

```csharp
_productRepo.GetAll()
    .Include(p => p.Category)
    .ToList();
```

only pulls products into memory at the `.ToList()` boundary — not before. Everything
chained before that call is still a pending SQL expression.


## Caching Strategy

```csharp
string cacheKey = term.Trim().ToLower();
if (_cache.TryGetValue(cacheKey, out var cachedResults)) return cachedResults;
_cache[cacheKey] = freshResults;
```

The current cache is a `Dictionary<string, List<T>>` scoped to the repository
instance. It is intentionally simple. For production it should be replaced with
`IMemoryCache` with a sliding expiration and eviction triggered on product
create/update/delete. For multi-server deployments, `IDistributedCache` backed
by Redis is the natural next step.

---

## What Would Change at Scale

| Current | At Scale |
|---|---|
| `Dictionary` cache | `IMemoryCache` with TTL + eviction hooks |
| In-memory Levenshtein | Elasticsearch with `fuzziness: AUTO` |
| Synchronous search | `async/await` throughout with `ToListAsync()` |
| Single server cache | Redis `IDistributedCache` |

---

## Summary

The central design decision — `IQueryable<T>` on a generic `IRepository<T>` — avoids
the maintenance burden of per-entity interfaces while keeping query execution efficient
at the database level. As the application adds new entities, no new repository
infrastructure is needed. Callers retain the flexibility to compose their own queries,
include related data only when needed, and defer materialisation until the right
boundary. The search engine and cache layer sit cleanly on top of this foundation,
each with a single responsibility that can be replaced independently as requirements
grow.

Video Demo: https://www.youtube.com/watch?v=55S1V9OHN-o