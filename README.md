# MVC Music Store Modernization

This project modernizes the legacy ASP.NET MVC Music Store sample application from a .NET Framework / SQL Server stack to an ASP.NET Core MVC / .NET 8 application backed by PostgreSQL.

## Selected Legacy Project

Legacy source:

https://github.com/sagulati/MvcMusicStore

MVC Music Store is a simple online music store application. The main domain entities are:

- Album: main product record
- Artist: performer or band linked to an album
- Genre: music category linked to an album
- Cart: temporary shopping cart records
- Order and OrderDetails: checkout and order line-item records

## Modernization Objective

The modernization exercise covers the following goals:

- Restore and inspect the legacy ASP.NET MVC / SQL Server application
- Rebuild the application using ASP.NET Core MVC / .NET 8
- Replace SQL Server runtime database dependency with PostgreSQL
- Use EF Core and Npgsql for PostgreSQL access
- Migrate catalog data from SQL Server to PostgreSQL
- Validate the modernized app workflows against PostgreSQL
- Implement authentication and admin authorization using ASP.NET Core Identity

## Technology Stack

### Legacy

- ASP.NET MVC / .NET Framework
- SQL Server
- Windows Server VM

### Modernized

- ASP.NET Core MVC
- .NET 8
- PostgreSQL 16
- Entity Framework Core
- Npgsql EF Core provider
- ASP.NET Core Identity

## Environment Setup

The legacy and modern runtimes were kept separated.

### Legacy Environment

- Windows Server 2019 VM
- SQL Server Developer Edition
- Legacy MVC Music Store restored and run locally
- Legacy database: `MusicStore`
- Host-to-VM SQL Server access configured through NAT port forwarding

### Modern Environment

- Windows 11 host
- ASP.NET Core MVC / .NET 8 application
- PostgreSQL 16 and pgAdmin
- EF Core with Npgsql provider
- ASP.NET Core Identity for login, register, logout, and admin role
- Credentials kept outside source code using User Secrets

## Database Migration

Catalog data migration was automated using scripts under:

```text
MigrationData/Scripts
```

Scripts included:

```text
01_Setup_SQLServer_Migration_User.sql
02_Export_AlbumModule_From_SQLServer.ps1
03_Import_AlbumModule_To_PostgreSQL.sql
```

Migration flow:

```text
SQL Server in legacy VM
--> CSV export
--> CSV staging
--> PostgreSQL import
--> EF Core
--> ASP.NET Core MVC pages
```

Migrated catalog counts:

```text
Albums: 246
Artists: 137
Genres: 10
```

The original legacy dataset had empty transactional tables for carts, orders, and order details, so final checkout/order rows were created through the modernized application during validation.

## Implemented Workflows

### Store Browse Workflow

Implemented routes:

```text
/
 /Store
 /Store/Browse?genre=Rock
 /Store/Details/{AlbumId}
```

Implemented behavior:

- Home page loads
- Store page displays genres
- Browse page filters albums by genre
- Album Details page displays album information
- Album data is read from PostgreSQL using EF Core/Npgsql

### Shopping Cart Workflow

Implemented routes:

```text
/ShoppingCart/AddToCart/{AlbumId}
/ShoppingCart
```

Implemented behavior:

- Add album to cart
- View shopping cart
- Display quantity, line total, and total
- Remove one item from cart
- Cart count displayed in navigation

### Checkout Workflow

Implemented routes:

```text
/Checkout
/Checkout/AddressAndPayment
/Checkout/Complete/{OrderId}
```

Implemented behavior:

- Checkout requires login
- Address and Payment page collects order details
- Promo code `FREE` is required
- Promo code validation is case-insensitive
- Orders are saved to PostgreSQL
- OrderDetails are saved with AlbumId, Quantity, and UnitPrice
- Cart is cleared after successful checkout
- Order Complete page displays the order number

### Authentication and Authorization

Implemented routes:

```text
/Identity/Account/Register
/Identity/Account/Login
/StoreManager
/Albums
/Artists
/Genres
```

Implemented behavior:

- Register
- Login
- Logout
- ASP.NET Core Identity integration
- Administrator role
- Role-protected StoreManager route
- Role-protected Album, Artist, and Genre management pages
- Non-admin users receive Access Denied for protected admin routes

## Validation Queries

Catalog validation:

```sql
SELECT 'Albums' AS tab_name, COUNT(*) AS cnt FROM "Albums"
UNION ALL
SELECT 'Artists', COUNT(*) FROM "Artists"
UNION ALL
SELECT 'Genres', COUNT(*) FROM "Genres";
```

Expected result:

```text
Albums: 246
Artists: 137
Genres: 10
```

Order validation:

```sql
SELECT "OrderId", "Username", "Email", "OrderDate", "Total"
FROM "Orders"
ORDER BY "OrderId" DESC;
```

Order detail validation:

```sql
SELECT 
    o."OrderId",
    o."Username",
    o."Email",
    od."AlbumId",
    a."Title",
    od."Quantity",
    od."UnitPrice",
    o."Total"
FROM "Orders" o
JOIN "OrderDetails" od ON o."OrderId" = od."OrderId"
JOIN "Albums" a ON od."AlbumId" = a."AlbumId"
ORDER BY o."OrderId" DESC;
```

## Security Notes

No real passwords are committed to the repository.

Sensitive values should be configured locally using .NET User Secrets.

Examples:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<LOCAL_POSTGRES_CONNECTION_STRING>"
dotnet user-secrets set "AdminSeed:Email" "admin@musicstore.com"
dotnet user-secrets set "AdminSeed:Password" "<LOCAL_ADMIN_PASSWORD>"
```

Migration scripts have been sanitized and use placeholders or prompt-based password input.

The repository excludes local build and environment files using `.gitignore`, including:

```text
.vs/
bin/
obj/
*.user
*.backup
*.zip
appsettings.Development.json
```

## Running Locally

1. Install prerequisites:

```text
Visual Studio 2022
.NET 8 SDK
PostgreSQL 16
pgAdmin
```

2. Clone the repository.

3. Configure the PostgreSQL connection string using User Secrets.

4. Apply EF Core migrations.

In Visual Studio Package Manager Console:

```powershell
Update-Database
```

5. Import catalog data using the scripts under:

```text
MigrationData/Scripts
```

6. Run the application from Visual Studio.

7. Open:

```text
https://localhost:7111/
```

## Demo Entry Points

```text
Home: https://localhost:7111/
Store: /Store
Cart: /ShoppingCart
Checkout: /Checkout
Admin: /StoreManager
Login: /Identity/Account/Login
Register: /Identity/Account/Register
```

## Current Status

Completed:

- Legacy app restored and inspected
- Modern ASP.NET Core MVC / .NET 8 app created
- PostgreSQL schema created through EF Core migrations
- Catalog migration flow automated and validated
- Store workflow implemented
- Cart workflow implemented
- Checkout workflow implemented
- Login/Register/Logout implemented
- Admin access protected with Administrator role
- Code pushed to GitHub

## Future Scope

- AWS deployment as future enhancement
- Additional UI refinement if required