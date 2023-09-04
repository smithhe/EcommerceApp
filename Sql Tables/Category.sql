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
