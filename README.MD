# Recipe App 

The Recipe App is an ASP.NET REST API designed to create, save, and share food recipes. MySQL is used as the database to store and manage the recipe data. The app allows users to perform various operations on the recipes, such as creating, reading, updating, and deleting them.

## Features

- User authentication system with login/register
- JWT tokens for API authentication
- CRUD functionality for users
- CRUD functionality for recipes 
- Swagger/OpenAPI documentation for API

## Usage

### Run Migrations and Add to Database

dotnet ef migrations add new

dotnet ef database update

### Run Development Server

dotnet build

dotnet run

## API Documentation

Swagger API schema and documentation available at:

http://localhost:5000/swagger

![Recipe App](swagger.png)