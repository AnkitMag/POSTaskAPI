POS Web API – README
This is a small Point of Sale (POS) Web API that lets you manage products, product types, users, and orders simply.

What this API does
Manage products and product types (create, read, update, delete).
​

Manage users and their orders with order details.

Protect endpoints with JWT authentication so only logged‑in users can access secure APIs.

Tech I used
ASP.NET Core Web API

Entity Framework Core with SQL Server

Generic Repository + Dependency Injection to keep the code clean and testable.

Async/await everywhere for better performance and responsiveness.

JWT tokens for login and authorization.

Main resources

/api/users – manage users.

/api/producttype – manage product categories.

/api/product – manage products.

/api/order – create and view orders (with order details included).

How authentication works
Client sends username and password to POST /api/auth/login.

API returns a JWT token if the login is valid.

Client sends this token in the Authorization: Bearer <token> header for protected endpoints.

How to run it
Update the SQL Server connection string in appsettings.json.

Apply EF Core migrations (dotnet ef database update).
​

Run the project (dotnet run).

Open the Swagger URL (shown in the console) to try the endpoints.
