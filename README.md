# Bakery API

Welcome to the Bakery API! This project provides an API for managing a bakery's products, categories, and options. The API is built with ASP.NET Core and Entity Framework Core.

## Table of Contents

- [Project Overview](#project-overview)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Database Migrations](#database-migrations)
  - [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
  - [Categories](#categories)
- [Contributing](#contributing)
- [License](#license)

## Project Overview

This project consists of the following key components:

- BakeryDbContext: The Entity Framework Core DbContext, representing the database.
- Models: Classes representing the entities (Category, Option, Product).
- DTOs: Data Transfer Objects for creating and updating entities.
- Endpoints: API endpoints for managing categories, products, and options.
- Repositories: Interfaces and implementations for data access.
- Mapping: Extension methods for mapping between entities and DTOs.

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQLite (for database)
- Swagger (for API documentation)

## Getting Started

### Prerequisites
- .NET 8 SDK
- A code editor or IDE (e.g., Visual Studio, Visual Studio Code)

### Installation
1. Clone the repository:
```
git clone https://github.com/your-username/bakery-api.git
cd bakery-api
```
2. Install the required packages:
```
dotnet restore
```
3. Database Migrations
Ensure you have the correct connection string in appsettings.json. Then, apply the database migrations:
```
dotnet ef database update
```
4. Running the Application
Run the application using the following command:
```
dotnet run
```
5. The API will be available at https://localhost:5001 (or another port if configured differently).

## API Endpoints

### Categories
- get all categories GET /categories
- get category by id GET /categories/{id}
- create a category POST /categories
- update a category PUT /categories/{id}
- delete a category DELETE /categories/{id}

### Options
- get all options GET /options
- get option by id GET /options/{id}
- create a option POST /options
- update a option PUT /options/{id}
- delete a option DELETE /options/{id}

### Products
- get all products GET /products
- get product by id GET /products/{id}
- create a product POST /products
- update a product PUT /products/{id}
- delete a product DELETE /products/{id}

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.