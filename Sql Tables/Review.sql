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
