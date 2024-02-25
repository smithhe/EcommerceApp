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