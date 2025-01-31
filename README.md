# MiniBank API

MiniBank API is a simple banking system that allows users to perform transactions such as sending money and reverting transactions. It follows a clean architecture approach and uses CQRS for handling commands and queries.

## Features
- User management
- Transactions (send money, revert transactions)
- Authorization service for transaction validation
- Notification service for transaction alerts

## Technologies Used
- .NET 9
- Entity Framework Core
- Mediator Pattern (MediatR)
- Docker & Docker Compose
- PostgreSQL
- xUnit & Moq for testing

## Setup and Running the Project
### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker & Docker Compose](https://www.docker.com/get-started)

### Running the Application
1. Clone the repository:
   ```sh
   git clone https://github.com/L968/MiniBank.git
   cd MiniBank
   ```
2. Start the services with Docker Compose:
   ```sh
   docker-compose up -d
   ```
3. Apply database migrations:
   ```sh
   dotnet ef database update
   ```
4. Run the application:
   ```sh
   dotnet run --project MiniBank.Api
   ```

## Running Tests
To run unit tests, execute:
```sh
 dotnet test
```

## API Endpoints
Once the application is running, you can access the API via Swagger:
```
http://localhost:5000/swagger/index.html
```

## Contributing
Feel free to open issues and pull requests to improve the project!

## License
This project is licensed under the MIT License.
