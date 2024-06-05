# Getting Started

To setup and run the project locally you will need to first install the prequisite software on your machine and then follow the instructions below

### Software Requirements:
* .Net 8 SDK 
* Docker

Optionally if you would like to have your containers hosted on a separate machine Portainer would be a good option to setup and manage the stack for this application.

### Additional Setup Requirements
* A PayPal account
    
    The current implemented method for checkout is through PayPal, in order to use the checkout system you will need to setup a dev account and get the credentials to make API calls as well as sandbox credentials to complete a checkout
* A mail server
    
    If you opt to not use the mailhog server below and want to have emails actually sent out you will need to set something up and get the credentials necessary for the secrets.json file in the worker project.

### Setup Steps
1) Open Powershell and move into the root directory of the project
    - Your shell path should look similar to `C:\EcommerceApp>` on windows
    - For `superior` Linux users it should look more like this `~/GitRepos/EcommerceApp$`
    <br/><br/>
2) Run the following command to set up a MySQL Docker Container
    ```bash
    docker run -d --name ecommerce-mailhog -p 1025:1025 -p 8025:8025 mailhog/mailhog
    docker run -d --name ecommerce-rabbit -e RABBITMQ_DEFAULT_USER=user -e RABBITMQ_DEFAULT_PASS=password -p 15672:15672 -p 5672:5672 rabbitmq:3-management
    docker run -d --name ecommerce-mysql -e MYSQL_ROOT_PASSWORD=password -e MYSQL_DATABASE=ecommerce -e MYSQL_USER=applicationuser -e MYSQL_PASSWORD=password -p 3306:3306 mysql:latest
    ```

3) Run the following command to install the dotnet tools needed for this repo
    ```bash
    dotnet tool restore
    ```
        
4) Update your `appsettings.Development.json` or file in the API project to have the following connection string <br/> (You *SHOULD* change the values to match your MySQL container setup)
    ```json
    "ConnectionStrings": {
        "DefaultConnection": "server=localhost;port=3306;database=ecommerce;user=applicationuser;password=password"
    }
    ```
5) You will need to use a program of your choice or bash into the container to access your MySql database to run the following snippet as a user with admin privileges
   ```sql
    SET GLOBAL event_scheduler = ON;
   ```
6) Add a secrets.json file to the API project and add paste the following into it<br/> Be sure to update the sections with the values for your project
    ```json
    {
        "JwtSettings": {
        "Key": "somevalue",
        },
        "Paypal": {
            "ClientId": "AnId",
            "Secret": "something",
        },
        "RabbitMQ": {
            "Username": "CHANGEME",
            "Password": "CHANGEME"
        }
    }
    ```
7) Add a secrets.json file to the Worker project and paste the following into it<br/> As with the API project be sure to update your values appropriately, if you are using mailhog you can skip the MailSettings object
    ```json
    {
        "RabbitMQ": {
            "Username": "CHANGEME",
            "Password": "CHANGEME"
        },
        "MailSettings": {
            "Host": "mailhog",
            "Port": 1025,
            "UserName": "CHANGEME",
            "Password": "CHANGEME"
        }
    }
    ```
8) (Mailhog Only)
9) Run the UI and API project together in an IDE of your choice

### Post Setup Instructions

1) You can now run the application, but you will need to go to the register page, you can either append the UI 
path with `/Register` or click login in the top right and click `Register here`

2) To add more Products with pictures you will need to set up your own S3 bucket and use the urls from that to 
show product pictures
