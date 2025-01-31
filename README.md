# Vertical Slice .NET Template

This repository provides a template for creating .NET projects using **Vertical Slice Architecture**, integrated with **Docker Compose**, **Entity Framework Core** and more. This template helps you quickly set up and organize your project with a modular structure and seamless database management.

## Technologies and patterns

- [Vertical Slice Architecture](https://www.milanjovanovic.tech/blog/vertical-slice-architecture) Structure your project by feature, making it easier to maintain and scale.
- [Docker Compose](https://docs.docker.com/compose/): Pre-configured for containerization, allowing you to run the entire project with one command.
- [MySQL](https://www.mysql.com/): The template uses MySQL as the default database, with easy setup through Docker.
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/): Simplified database management with Entity Framework Core, allowing you to apply and manage migrations effortlessly.
- [Health Checks](https://www.nuget.org/packages/AspNetCore.HealthChecks.UI.Client): Integrated health checks for monitoring the application state.
- [FluentValidation](https://fluentvalidation.net/): Provides a clean and expressive way to validate models.
- [MediatR](https://github.com/jbogard/MediatR): Enables in-process messaging for better separation of concerns.
- [xUnit](https://xunit.net/), [Moq](https://github.com/moq): Testing libraries

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (with Docker Compose)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/L968/MiniBank.git

cd MiniBank
```

### 2. Install the Template

To install the template globally, run the following command:

```bash
dotnet new --install .
```

### 3. Create a New Project

To create a new project using the Vertical Slice template, use the command:

```bash
cd ..

dotnet new verticalslice-template -o "YourProjectName"
```

## Using this Project

### 1. Build and Run the Application

To run the application with Docker Compose, use the following command:

```bash
cd YourProjectName

docker-compose up
```

This command will build the Docker containers, set up the application, and run it along with the MySQL database.

**Note:** When running `docker-compose up`, it may take a moment for the MySQL container to initialize completely. Ensure that the database is up and running before proceeding to the next step.

### 2. Apply Database Migrations

Once the application and database containers are running, you need to apply the EF Core migrations to initialize the database schema.

1. Open Visual Studio and load your project.

2. Open the Package Manager Console:
   - Go to the **Tools** menu.
   - Select **NuGet Package Manager**.
   - Click on **Package Manager Console**.

3. Execute the following command in the console:
```bash
Update-Database
```

This will apply any pending migrations and create the necessary tables in the MySQL database.

### 3. Access the Application

Once the application is running, it can be accessed at:

```text
http://localhost:5000
```

(Adjust the port if needed, depending on your configuration in docker-compose.yml).

## License

This project is licensed under the [MIT License](LICENSE.txt).
