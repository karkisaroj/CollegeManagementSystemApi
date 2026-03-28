# College Management System API

A RESTful API built with .NET 9.0 and ASP.NET Core for managing college data.

## Features

- Student management (CRUD operations)
- Course management with modules
- Instructor assignments
- Enrollment tracking

## Technologies

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Scalar API Documentation

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL database

### Run the Application

```bash
dotnet restore
dotnet build
dotnet run
```

### API Documentation

- Swagger: `/swagger`
- Scalar UI: `/scalar/v1`

## Endpoints

| Endpoint | Description |
|----------|-------------|
| `/api/students` | Student operations |
| `/api/courses` | Course operations |