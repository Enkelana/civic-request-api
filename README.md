CivicRequest API
Backend i sistemit CivicRequest — Platforma e Kërkesave të Qytetarëve
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-blue)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-Core-green)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED)
![Deploy](https://img.shields.io/badge/Deploy-Render.com-46E3B7)
---
Përshkrimi
CivicRequest API është një RESTful Web API i ndërtuar me ASP.NET Core (.NET 10) që mundëson menaxhimin e kërkesave të qytetarëve ndaj institucioneve publike shqiptare. Sistemi ofron autentifikim të sigurt me JWT, njoftime automatike me email, dhe është i deployuar plotësisht në cloud.
🌐 API Live: https://civic-request-api.onrender.com
---
🚀 Stack Teknologjik
Teknologjia	Versioni	Roli
ASP.NET Core Web API	.NET 10	Framework kryesor
Entity Framework Core	Latest	ORM — Database access
SQL Server LocalDB	2022	Database lokale (development)
PostgreSQL	16	Database online (production)
ASP.NET Identity	Latest	Menaxhim zyrtarësh
JWT Bearer	Latest	Autentifikim
MailKit	Latest	Email SMTP (Gmail)
Scalar	Latest	API Dokumentacion
Docker	Latest	Containerization
---
📁 Struktura e Projektit
```
CivicRequest.API/
├── Controllers/
│   ├── AuthController.cs        # Login, Register, Profile, ChangePassword
│   ├── CitizensController.cs    # CRUD për qytetarë
│   ├── RequestsController.cs    # CRUD + Search për kërkesa
│   └── CategoriesController.cs # Lexim kategorish
├── Models/
│   ├── Citizen.cs              # Modeli i qytetarit
│   ├── Request.cs              # Modeli i kërkesës
│   ├── Category.cs             # Modeli i kategorisë
│   └── Officer.cs              # Modeli i zyrtarit (Identity)
├── Data/
│   └── AppDbContext.cs         # Database Context + Seed Data
├── DTOs/
│   ├── AuthDto.cs              # Register, Login, ChangePassword
│   └── RequestDto.cs           # Create, Update Request
├── Services/
│   └── EmailService.cs         # MailKit SMTP Service
├── Migrations/                 # EF Core Migrations
├── Program.cs                  # Konfigurimi i aplikacionit
├── appsettings.json            # Konfigurimet
└── Dockerfile                  # Docker configuration
```
---
🔗 API Endpoints
🔐 Auth
Method	Endpoint	Përshkrimi	Auth
POST	`/api/auth/register`	Regjistro zyrtar të ri	❌
POST	`/api/auth/login`	Login dhe merr JWT Token	❌
GET	`/api/auth/profile`	Merr të dhënat e profilit	✅
PUT	`/api/auth/change-password`	Ndrysho fjalëkalimin	✅
👥 Citizens
Method	Endpoint	Përshkrimi	Auth
GET	`/api/citizens`	Merr të gjithë qytetarët	✅
GET	`/api/citizens/{id}`	Merr një qytetar	✅
POST	`/api/citizens`	Krijo qytetar të ri	✅
PUT	`/api/citizens/{id}`	Përditëso qytetarin	✅
📋 Requests
Method	Endpoint	Përshkrimi	Auth
GET	`/api/requests`	Merr të gjitha kërkesat	✅
GET	`/api/requests/{id}`	Merr një kërkesë	✅
GET	`/api/requests/search`	Kërko dhe filtro	✅
GET	`/api/requests/status/{status}`	Filtro sipas statusit	✅
POST	`/api/requests`	Krijo kërkesë të re	✅
PUT	`/api/requests/{id}/status`	Përditëso statusin	✅
DELETE	`/api/requests/{id}`	Fshi kërkesën	✅
📂 Categories
Method	Endpoint	Përshkrimi	Auth
GET	`/api/categories`	Merr të gjitha kategorite	❌
GET	`/api/categories/{id}`	Merr një kategori	❌
---
⚙️ Instalim Lokal
Kërkesat
.NET 10 SDK
SQL Server LocalDB
Visual Studio 2022 ose VS Code
Hapat
1. Clone projektin
```bash
git clone https://github.com/Enkelana/civic-request-api.git
cd civic-request-api
```
2. Konfigurojë `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CivicRequestDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_HERE",
    "Issuer": "CivicRequest.API",
    "Audience": "CivicRequest.Client"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "CivicRequest",
    "Password": "your-app-password"
  }
}
```
3. Ekzekuto Migrations
```bash
dotnet ef database update
```
4. Starto projektin
```bash
dotnet run
```
5. Hap API Dokumentacionin
```
http://localhost:5280/scalar/v1
```
---
🐳 Docker
```bash
docker build -t civicrequest-api .
docker run -p 8080:8080 civicrequest-api
```
---
🌐 Deploy (Render.com)
Projekti është i konfiguruar për deploy automatik në Render.com me Docker.
Environment Variables të nevojshme:
```
ConnectionStrings__DefaultConnection = postgresql://...
Jwt__Key                             = your-secret-key
Jwt__Issuer                          = CivicRequest.API
Jwt__Audience                        = CivicRequest.Client
ASPNETCORE_ENVIRONMENT               = Production
ASPNETCORE_URLS                      = http://0.0.0.0:8080

📊 Modelet e Databazës
```
Category          Citizen           Request           Officer (Identity)
─────────         ────────          ───────           ──────────────────
Id (PK)           Id (PK)           Id (PK)           Id (PK)
Name              FullName          Title             FullName
Description       Email             Description       Email
                  Phone             Status (enum)     Role
                  NationalId        CitizenId (FK)    PasswordHash
                  CreatedAt         CategoryId (FK)   CreatedAt
                                    OfficerNotes
                                    CreatedAt
                                    UpdatedAt

👤 Autor
Enkelana 

🔗 Linke
🌐 Frontend: https://civic-request-frontend.vercel.app
📡 API Live: https://civic-request-api.onrender.com
💻 Frontend Repo: https://github.com/Enkelana/civic-request-frontend
