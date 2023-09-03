CREATE TABLE Review (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
    UserId BINARY(16) NOT NULL,
    Stars INT NOT NULL,
    Comments TEXT,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    CONSTRAINT FK_Product FOREIGN KEY (ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_User FOREIGN KEY (UserId) REFERENCES User(Id),
    CONSTRAINT CHK_CreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_LastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);
