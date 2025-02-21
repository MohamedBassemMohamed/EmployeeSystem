# Employee Management System
├── Core/                    # Domain entities and interfaces
│   ├── Entities/
│   └── Interfaces/
├── Application/            # Business logic and services
│   ├── DTOs/
│   └── Services/
├── Infrastructure/         # Data access and external services
│   └── Data/
└── API/                   # Controllers and configuration
    └── Controllers/



   ## Overview
   This is a simple Employee Management System built with .NET Core for the backend.

   ## Setup
   1. Clone the repository.
   2. Run dotnet restore to restore the packages.
   3. Update the database connection string in appsettings.json.
   4. Run dotnet ef database update to apply migrations.
   5. Run dotnet run to start the API.

   ## API Documentation
   The API documentation is available via Swagger UI at https://localhost:5001/swagger.

   ## Testing
   Unit tests are available in the Tests folder. Run dotnet test to execute the tests.
