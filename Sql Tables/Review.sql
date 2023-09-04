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
    CONSTRAINT FK_RProduct FOREIGN KEY (ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_RUser FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    CONSTRAINT CHK_RCreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_RLastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);
