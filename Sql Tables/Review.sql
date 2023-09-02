CREATE TABLE Review (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
    Stars INT NOT NULL,
    Comments TEXT,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    FOREIGN KEY (ProductId) REFERENCES Product(Id),
    CONSTRAINT CHK_CreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_LastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);
