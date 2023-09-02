CREATE TABLE OrderItem (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProductId INT NOT NULL,
	OrderId INT NOT NULL,
    Quantity INT NOT NULL,
    CreatedBy VARCHAR(255) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    LastModifiedBy VARCHAR(255),
    LastModifiedDate DATETIME,
    CONSTRAINT FK_Product FOREIGN KEY (ProductId) REFERENCES Product(Id),
	CONSTRAINT FK_Product FOREIGN KEY (OrderId) REFERENCES Order(Id),
    CONSTRAINT CHK_CreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_LastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);
