# SportComplexApp — Web Application for Sports Complex Management

SportComplexApp is a modern ASP.NET Core solution that helps manage sports, facilities, trainers, tournaments, spa procedures, and reservations. The solution includes a **MVC website** (`SportComplexApp.Web`) and a **Web API** (`SportComplexApp.WebAPI`).

## Features

**Sports Management**

* 🔍 Browse & filter sports by duration.
* 💰 See price, duration, image, and facility.
* 🛠 Admin: add, edit, delete sports.

**Facilities**

* 🏟 View facilities used by sports and events.
* 🛠 Administer facilities.

**Tournaments**

* 🏆 Explore tournaments and details.
* 🛠 Admin: CRUD operations.

**Spa Procedures**

* 💆 Browse procedures with descriptions and pricing.
* 🕒 Book and manage spa appointments.
* 🛠 Admin: CRUD operations.

**Trainers**

* 👩‍🏫 View trainer profiles and specializations.
* 🛠 Admin: CRUD operations.

**User Management (Admin Area)**

* 👥 View users and manage roles.
* 🔒 Assign/remove roles; delete users when needed.

## API (WebAPI)

* Public **GET** endpoints for Sports:

  * `GET /Tournament` — list of all tournaments.
  * `POST /Tournament` - create tournament.
  * `PUT /Tournamet` - edit tournament.
  * `GET /Tournament/{id}` — get tournament by id.
  * `DELETE /Tournament/{id}` - delete tournament
* Swagger UI: `https://localhost:{port}/swagger`.

## Technologies

**Backend**

* 🌐 ASP.NET Core 8 (MVC + Web API), Entity Framework Core (SQL Server), ASP.NET Core Identity.
* 📄 Swagger / Swashbuckle for API docs.

**Frontend**

* 🎨 Razor Views (HTML, CSS, JavaScript, Bootstrap).

**Database**

* 💾 SQL Server.

## Installation

1. **Prerequisites**: .NET 8 SDK, SQL Server.
2. **Configure connection string** in `appsettings.Development.json` for both `Web` and `WebAPI` projects (key: `ConnectionStrings:SQLServer`).
3. **Apply migrations** from the solution root:

```bash
dotnet ef database update --project SportComplexApp.Data --startup-project SportComplexApp.Web
```

4. **Run**

```bash
# MVC
cd SportComplexApp.Web
dotnet run

# Web API (Swagger)
cd ../SportComplexApp.WebAPI
dotnet run
```

5. **Open**

* MVC: `https://localhost:{port}`
* API docs: `https://localhost:{port}/swagger`

## Important Notes

* 🔑 Development admin credentials should be stored via **User Secrets** (or `appsettings.*` for dev only). Change them for production.
* 🔒 Security basics: HTTPS, EF Core (parameterized), Anti-forgery in MVC forms, Identity roles for admin features.
* 🌍 If a SPA/mobile client will call the API from another domain, enable CORS in `WebAPI`.

## License

For educational/demo purposes. Adjust for production use as needed.
