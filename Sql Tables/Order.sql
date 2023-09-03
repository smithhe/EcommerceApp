CREATE TABLE Order (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId BINARY(16) NOT NULL,
    Status VARCHAR(255) NOT NULL,
    Total DOUBLE NOT NULL,
    CONSTRAINT FK_User FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT CHK_CreatedDate CHECK (CreatedDate <= LastModifiedDate),
    CONSTRAINT CHK_LastModifiedDate CHECK (LastModifiedDate IS NULL OR CreatedDate <= LastModifiedDate)
);