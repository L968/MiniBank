# MiniBank API

MiniBank API is a simple banking system that allows users to perform transactions such as sending money and reverting transactions. It follows a clean architecture approach and uses CQRS for handling commands and queries.

## Features
- User management
- Transactions (send money, revert transactions)
- Authorization service for transaction validation
- Notification service for transaction alerts

## Technologies Used
- .NET 9
- .NET Aspire  
- Entity Framework Core
- MySQL
- Mediator Pattern (MediatR)
- Docker
- RabbitMQ & MassTransit for async communication
- xUnit, Moq & Bogus for testing

## Setup and Running the Project
### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started)

### Running the Application
1. Clone the repository:
   ```sh
   git clone https://github.com/L968/MiniBank.git
   cd MiniBank
   ```
2. **Ensure Docker is running:**  
   Before starting the application, make sure **Docker Desktop** (Windows/macOS) or the **Docker service** (Linux) is running on your system.
   
3. Run the application using .NET Aspire:
   ```sh
   dotnet run --project MiniBank.AppHost
   ```

## Running Tests
To run unit tests, execute:
```sh
 dotnet test
```

## API Endpoints  
Once the application is running, you can access the API via Scalar in the **MiniBank.Api** project from the .NET Aspire dashboard.  

## Contributing
Feel free to open issues and pull requests to improve the project!

## License
This project is licensed under the MIT License.
