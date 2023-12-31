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