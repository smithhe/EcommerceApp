use ecommerce;

#Create Tables For the Database
#-----------------------------------------------------------------------------------------------------------------------
CREATE TABLE Category (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Summary VARCHAR(50) NOT NULL,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    CONSTRAINT CHK_CCreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_CLastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);

CREATE TABLE Product (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT NOT NULL,
    Price DOUBLE NOT NULL,
    AverageRating DOUBLE NOT NULL,
    QuantityAvailable INT NOT NULL,
    ImageUrl TEXT NOT NULL,
    CategoryId INT NOT NULL,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    CONSTRAINT FK_PCategory FOREIGN KEY (CategoryId) REFERENCES Category(Id) ON DELETE CASCADE,
	CONSTRAINT CHK_PCreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_PLastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);

CREATE TABLE CartItem (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
    UserId VARCHAR(255) NOT NULL,
    Quantity INT NOT NULL,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    CONSTRAINT FK_CIProduct FOREIGN KEY (ProductId) REFERENCES Product(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CIUser FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_CICreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_CILastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);

CREATE TABLE `Order` (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL,
    Status VARCHAR(255) NOT NULL,
    Total DOUBLE NOT NULL,
    PayPalRequestId VARCHAR(255),
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    INDEX(PayPalRequestId),
    CONSTRAINT FK_OUser FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
	CONSTRAINT CHK_OCreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_OLastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);

CREATE TABLE OrderItem (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
	OrderId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DOUBLE NOT NULL,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
	CONSTRAINT FK_OIOrder FOREIGN KEY (OrderId) REFERENCES `Order`(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_OICreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_OILastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);

CREATE UNIQUE INDEX IX_UniqueUserName ON AspNetUsers(UserName);

CREATE TABLE Review (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
    UserName VARCHAR(256) NOT NULL,
    Stars INT NOT NULL,
    Comments TEXT,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    CONSTRAINT FK_RProduct FOREIGN KEY (ProductId) REFERENCES Product(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RUser FOREIGN KEY (UserName) REFERENCES AspNetUsers(UserName) ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT CHK_RCreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_RLastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);

CREATE TABLE OrderKey (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    OrderToken VARCHAR(50) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

#Create an event to delete Order Keys that are older than 3 hours
#-----------------------------------------------------------------------------------------------------------------------
SET GLOBAL event_scheduler = ON;
CREATE EVENT DeleteOrderKey
ON SCHEDULE EVERY 1 MINUTE
DO
    DELETE FROM OrderKey WHERE CreatedAt < (NOW() - INTERVAL 3 HOUR);



#Insert Values into the Database
#-----------------------------------------------------------------------------------------------------------------------
INSERT INTO Category (Name, Summary, CreatedBy, CreatedDate) VALUES ('Laptops', 'Explore our range of laptops.', 'Harold', CURRENT_DATE);
INSERT INTO Category (Name, Summary, CreatedBy, CreatedDate) VALUES ('Phones', 'Discover the latest smartphones.', 'Harold', CURRENT_DATE);
INSERT INTO Category (Name, Summary, CreatedBy, CreatedDate) VALUES ('Tablets', 'Browse our collection of tablets.', 'Harold', CURRENT_DATE);

INSERT INTO Product (Name, Description, Price, AverageRating, QuantityAvailable, ImageUrl, CategoryId, CreatedBy, CreatedDate)
VALUES ('Laptop 1', 'This is a killer laptop that can handle all your home needs', 299.99, 4, 5, 'https://smith-ecommerce-app.s3.amazonaws.com/laptop1.jpg', 1, 'Harold', CURRENT_DATE);

INSERT INTO Product (Name, Description, Price, AverageRating, QuantityAvailable, ImageUrl, CategoryId, CreatedBy, CreatedDate)
VALUES ('Laptop 2', 'This is a killer laptop that can handle all your home needs', 499.99, 4.7, 7, 'https://smith-ecommerce-app.s3.amazonaws.com/laptop2.jpg', 1, 'Harold', CURRENT_DATE);

INSERT INTO Product (Name, Description, Price, AverageRating, QuantityAvailable, ImageUrl, CategoryId, CreatedBy, CreatedDate)
VALUES ('Laptop 3', 'This is a killer laptop that can handle all your home needs', 999.99, 4.7, 2, 'https://smith-ecommerce-app.s3.amazonaws.com/laptop3.jpg', 1, 'Harold', CURRENT_DATE);


