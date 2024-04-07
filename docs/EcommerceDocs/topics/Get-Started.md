# Local Setup
### Software Requirements:
* .Net 8 SDK 
* Docker

### Setup Steps
1) Open Powershell and move into the Persistence project directory
    - Your shell path should look similar to `C:\EcommerceApp/Infrastructure/Ecommerce.Persistence>`
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
5) You will need to use a program of your choice or bash into the container to access your MySql database to run the following snippet as a user with admin privileges
   <code-block lang="sql">
      SET GLOBAL event_scheduler = ON;
   </code-block>
6) Add a secrets.json file to the API project and add paste the following into it and then update the value to a random key of your choosing <br/> (can be any value you want)
    <code-block lang="json">
        {
            "JwtSettings": {
                "Key": "PUTSOMETHINGHERE"
            }
        }
    </code-block>
7) Run the UI and API project together in an IDE of your choice

### Post Setup Instructions

1) You can now run the application, but you will need to go to the register page, you can either append the UI 
path with `/Register` or click login in the top right and click `Register here`

2) To add more Products with pictures you will need to set up your own S3 bucket and use the urls from that to 
show product pictures
