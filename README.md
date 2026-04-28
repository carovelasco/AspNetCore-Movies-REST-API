# AspNetCore Movies REST API

## Overview
RESTful API built with ASP.NET Core (.NET 9) for managing movies, users, and authentication.  
This project demonstrates modern backend development practices including clean architecture, secure authentication, and scalable API design.

## Features

- JWT Authentication and Authorization with ASP.NET Identity  
- Clean Architecture (Controllers, Services, Repository Pattern)  
- Entity Framework Core with SQL Server  
- DTOs with AutoMapper  
- Minimal APIs and MVC Controllers  
- Swagger / OpenAPI Documentation  
- Input validation and global error handling  
- Pagination, filtering, and optimized queries  

## Tech Stack

- Backend: ASP.NET Core (.NET 9)  
- Language: C#  
- Database: SQL Server  
- ORM: Entity Framework Core  
- Authentication: JWT + ASP.NET Identity  
- Documentation: Swagger (Swashbuckle)  

## Architecture

Controllers -> Services -> Repository -> Database

- Controllers: Handle HTTP requests  
- Services: Business logic  
- Repositories: Data access abstraction  
- DTOs: Data transfer between layers  

## Authentication

- JWT-based authentication  
- Role-based authorization  
- Secure endpoints with token validation  

## API Endpoints

GET /api/movies - Get all movies  
GET /api/movies/{id} - Get movie by ID  
POST /api/movies - Create new movie  
PUT /api/movies/{id} - Update movie  
DELETE /api/movies/{id} - Delete movie  
POST /api/auth/login - Authenticate user  

## Getting Started

### Clone repository
git clone https://github.com/your-username/AspNetCore-Movies-REST-API.git

### Configure database
Update connection string in appsettings.json

### Apply migrations
dotnet ef database update

### Run project
dotnet run
