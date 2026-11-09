 # RaceDay System

## 1. Project Overview

RaceDay System is a web-based application designed to manage road race events. The system aims to make it easier for participants to register for events and for organisers to manage race information.

The application is developed using ASP.NET Core and C#.

## 2. Technologies Used

The following technologies are used in this project:

- Visual Studio 2022
- C#
- ASP.NET Core .NET 8
- SQL Server LocalDB
- Entity Framework Core
- HTML and CSS
- RESTful Web API

## 3. Project Structure

The solution contains three main projects:

RaceDay.Api

This project contains the Web API, database connection, models, controllers, and application logic.

RaceDay.Mvc

This project is intended to contain the user interface of the application. It communicates with the API using HTTP requests.

RaceDay.Tests

This project contains unit tests used to check the functionality of the application.

4. System Requirements

To run this project, you need:

- Windows operating system
- Visual Studio 2022
- .NET 8 SDK
- SQL Server LocalDB
- Internet connection for downloading required NuGet packages

5. Database Configuration

The system uses SQL Server LocalDB as its database.

The connection string is configured in the `appsettings.json` file.

Example connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RaceDayDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Make sure SQL Server LocalDB is installed before running the application.

6. How to Run the Project

Follow these steps to run the project:

1. Open Visual Studio 2022.
2. Open the RaceDay System solution.
3. Make sure the solution targets .NET 8.
4. Check that all required NuGet packages are installed.
5. Check the database connection string in `appsettings.json`.
6. Build the solution to check for errors.
7. Set `RaceDay.Api` as the startup project.
8. Run the application.
9. Open the API URL displayed in the Visual Studio output window.

7. Testing

The project includes a separate `RaceDay.Tests` project.

The tests can be executed through Visual Studio:

1. Open Test Explorer.
2. Build the solution.
3. Select the tests you want to run.
4. Click Run All Tests.
5. Check the results for passed or failed tests.

 8. Important Notes

- The API project contains the Part 2 development work.
- The MVC project is currently awaiting implementation.
- The test project is maintained separately from the main application.
- The database must be configured correctly before running database-related functionality.

 9. Conclusion

The RaceDay System project demonstrates the development of a web-based event management application using ASP.NET Core, C#, and SQL Server.

The project is structured to separate the API, user interface, and testing components, making it easier to maintain and develop the application.

 
