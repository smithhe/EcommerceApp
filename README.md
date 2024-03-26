# Ecommerce App
A basic .Net project meant to demonstrate coding practices and knowledge of .Net technologies such as Blazor, .Net API and more

## ***Disclaimer***
This is app is not finished and is still being updated but following the instructions below will help you run locally

# Local Setup
### Software Requirements:
- .Net 7 SDK
- Docker

### Setup Steps
1) Open Powershell and move into the Identity project directory
    - Your shell path should look similar to `C:\EcommerceApp\Infrastructure\Ecommerce.Identity>`
2) Run the following command to setup a MySQL Docker Container

        docker run --name ecommerce-mysql -d -p 3306:3306 -e MYSQL_ROOT_PASSWORD=password -e MYSQL_DATABASE=ecommerce -e MYSQL_USER=applicationuser -e MYSQL_PASSWORD=Welcome1! mysql:latest
3) Run the following command to install the dotnet tools needed for this repo

        dotnet tool restore
4) Update your appsettings.Development.json file in the API project to have the following connection string <br/> (You *SHOULD* change the values to match your MySQL container setup)
   ```
   "ConnectionStrings": {
     "DefaultConnection": "server=localhost;port=3306;database=ecommerce;user=applicationuser;password=Welcome1!"
   }
   ```
5) Run the following command to update the database

       dotnet ef --startup-project ../../Api/Ecommerce.Api/ database update
6) Using a program of your choice, connect to the MySQL container and execute the SQL Statements in InitSql.sql with the root user of your MySql Instance

7) Add a secrets.json file to the API project and add paste the following into it and then update the value to a random key of your choosing <br/> (can be any value you want) 
   ```
   {
     "JwtSettings": {
       "Key": "PUTSOMETHINGHERE"
     }
   }
   ```
8) Run the UI and API project together in an IDE of your choice

9) You can now run the application but you will need to go to the register page, you can ether append the UI path with `/Register` or click login in the top right and click `Register here`
10) To add more Products with pictures you will need to setup your own S3 bucket and use the urls from that to show product pictures
