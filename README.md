# ZaroShop

A full-stack e-commerce application built with **ASP.NET Core** (.NET 8) and **Angular** (standalone).

---

## Prerequisites

Make sure the following are installed before getting started:

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| Node.js | 18.x+ | https://nodejs.org |
| npm | 9.x+ | Bundled with Node.js |
| Angular CLI | 17.x+ | `npm install -g @angular/cli` |
| SQL Server | 2019+ / LocalDB | https://www.microsoft.com/en-us/sql-server |

---

## Project Structure

```
ZaroShop/
├── ZaroShop.Server/          # ASP.NET Core Web API
│   ├── Controllers/
│   ├── Repositories/
│   ├── Utilities/            # SearchEngine<T> lives here
│   ├── appsettings.json
│   └── ZaroShop.Server.csproj
├── zaroshop.client/          # Angular frontend
│   ├── src/
│   ├── angular.json
│   └── package.json
└── ZaroShop.sln
```

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/zaroshop.git
cd zaroshop
```

### 2. Configure the Database

Open `ZaroShop.Server/appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ZaroShopDb;Trusted_Connection=True;"
}
```

For SQL Server with credentials:

```json
"DefaultConnection": "Server=localhost;Database=ZaroShopDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### 3. Apply Database Migrations

```bash
cd ZaroShop.Server
dotnet ef database update
```

> If you don't have the EF CLI tool yet:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### 4. Install Frontend Dependencies

```bash
cd ../zaroshop.client
npm install
```

---

## Running Locally

### Run Both Together (Recommended)

ASP.NET Core is configured to proxy Angular in development. From the solution root:

```bash
cd ZaroShop.Server
dotnet run
```

The app will be available at:
- **API + SPA proxy:** `https://localhost:7170`
- **Swagger UI:** `https://localhost:7170/swagger`

The Angular dev server starts automatically via the SPA proxy middleware.

---