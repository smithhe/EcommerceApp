# Local Setup
### Software Requirements:
- .Net 7 SDK
- Docker

### Setup Steps
1) Open Powershell and move into the Identity project directory
    - Your shell path should look similar to `C:\EcommerceApp\Infrastructure\Ecommerce.Identity>`
2) Run the following command to set up a MySQL Docker Container
    <code-block lang="bash">
        docker run --name ecommerce-mysql -d -p 3306:3306 -e MYSQL_ROOT_PASSWORD=password -e MYSQL_DATABASE=ecommerce -e MYSQL_USER=applicationuser -e MYSQL_PASSWORD=Welcome1! mysql:latest
    </code-block>

3) Run the following command to install the dotnet tools needed for this repo
    <code-block lang="bash">
        dotnet tool restore
    </code-block>
        
4) Update your `appsettings.Development.json` file in the API project to have the following connection string <br/> (You *SHOULD* change the values to match your MySQL container setup)
    <code-block lang="json">
        "ConnectionStrings": {
            "DefaultConnection": "server=localhost;port=3306;database=ecommerce;user=applicationuser;password=Welcome1!"
        }
    </code-block>
5) Run the following command to update the database
    <code-block lang="bash">
       dotnet ef --startup-project ../../Api/Ecommerce.Api/ database update
    </code-block>
6) Using a program of your choice, connect to the MySQL container and execute the SQL Statements in InitSql.sql with the root user of your MySql Instance

7) Add a secrets.json file to the API project and add paste the following into it and then update the value to a random key of your choosing <br/> (can be any value you want)
    <code-block lang="json">
        {
            "JwtSettings": {
                "Key": "PUTSOMETHINGHERE"
            }
        }
    </code-block>
8) Run the UI and API project together in an IDE of your choice

9) You can now run the application, but you will need to go to the register page, you can either append the UI 
path with `/Register` or click login in the top right and click `Register here`

10) To add more Products with pictures you will need to set up your own S3 bucket and use the urls from that to 
show product pictures
