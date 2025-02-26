# IT Helpdesk API

This project is a simple CRUD web API for managing helpdesk cases. It allows users to create, read, update, and delete helpdesk cases, providing a basic framework for an IT helpdesk system.

## Project Structure

```
it-helpdesk-api
├── src
│   ├── Controllers
│   │   └── HelpdeskController.cs
│   ├── Models
│   │   └── HelpdeskCase.cs
│   ├── Services
│   │   └── HelpdeskService.cs
│   ├── Program.cs
│   └── Startup.cs
├── appsettings.json
├── appsettings.Development.json
├── it-helpdesk-api.csproj
└── README.md
```

## Getting Started

### Prerequisites

- .NET SDK (version 6.0 or later)
- An Azure account for hosting the API

### Installation

1. Clone the repository:
   ```
   git clone <repository-url>
   cd it-helpdesk-api
   ```

2. Restore the dependencies:
   ```
   dotnet restore
   ```

3. Run the application:
   ```
   dotnet run --project src/it-helpdesk-api.csproj
   ```

### API Endpoints

- **GET /api/helpdesk**: Retrieve all helpdesk cases.
- **GET /api/helpdesk/{id}**: Retrieve a specific helpdesk case by ID.
- **POST /api/helpdesk**: Create a new helpdesk case.
- **PUT /api/helpdesk/{id}**: Update an existing helpdesk case.
- **DELETE /api/helpdesk/{id}**: Delete a helpdesk case.

### Deployment

To deploy the API to Azure, follow these steps:

1. Create an Azure App Service.
2. Publish the application using the Azure CLI or Visual Studio.
3. Configure the connection strings and application settings in the Azure portal.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License

This project is licensed under the MIT License. See the LICENSE file for details.