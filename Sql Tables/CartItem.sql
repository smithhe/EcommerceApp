CREATE TABLE CartItem (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
    UserId BINARY(16) NOT NULL,
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