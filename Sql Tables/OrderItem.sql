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
    CONSTRAINT FK_OIProduct FOREIGN KEY (ProductId) REFERENCES Product(Id) ON DELETE CASCADE,
	CONSTRAINT FK_OIOrder FOREIGN KEY (OrderId) REFERENCES Order(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_OICreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_OILastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);
