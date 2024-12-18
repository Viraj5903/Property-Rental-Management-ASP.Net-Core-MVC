# Property Rental Management ASP.Net Core MVC

## Overview
The Property Rental Management System (PRMS) is a web-based application designed to streamline the management of rental properties. The system allows property owners, managers, and tenants to efficiently handle property listings, rental agreements, maintenance requests, appointments, and message management.

## Software Requirements
- Visual Studio 2022
- SQL Server Express 2022 or SQL Server Express 2019
- SQL Server Management Studio/ Azure Data Studio
- ASP.Net Core MVC
- Entity Framework

## Project Setup:

1. **Clone the repository to your local machine**.
    - To clone run: 
      ```
      git clone https://github.com/Viraj5903/Property-Rental-Management-ASP.Net-Core-MVC.git
      ```

2. **Open the project in Visual Studio 2022**.
   - After cloning, open the project in Visual Studio 2022.

3. **Install the required NuGet packages**.
    - Open the NuGet Package Manager Console in Visual Studio by going to Tools > NuGet Package Manager > Package Manager Console.
    - Run the following commands in NuGet Package Manager Console to install the necessary packages:
      - ```
        Install-Package Microsoft.EntityFrameworkCore
        Install-Package Microsoft.EntityFrameworkCore.SqlServer
        Install-Package Microsoft.EntityFrameworkCore.Tools
        Install-Package Microsoft.EntityFrameworkCore.Proxies
        ```

4. **Create the Database on Your Local Machine**
   - Open SQL Server Management Studio or Azure Data Studio.
   - Run the `PropertyRentalManagementDB.sql` SQL script to create the database and tables.

5. **Configure the database connection**
   - Open the appsettings.json file located in the root of the project. This file contains the connection string for your database.
   - Update the ConnectionStrings section to reflect your local SQL Server configuration. You will need to configure the PropertyRentalManagementDB connection string with the appropriate values for your environment.
     - Here is an example of how the ConnectionStrings section should look:
        ```json
        "ConnectionStrings": {
        "PropertyRentalManagementDB": "Server=<ServerName>;Initial Catalog=PropertyRentalManagementDB;User=<Username>;Password=<Password>;Integrated Security=False;TrustServerCertificate=True;"
        }
        ```

## Running the Application
- After completing the setup steps, build the project in Visual Studio by pressing Ctrl + Shift + B.
- Press F5 or click on Start Debugging to run the application locally in your browser.

## Additional Information
For more detailed information, including the database schema, project implementation, and other important details, please refer to the `Project Report.pdf` file located in the root directory of the repository.