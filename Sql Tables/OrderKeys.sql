CREATE TABLE OrderKey (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    OrderToken VARCHAR(255) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

#Create an event to delete Order Keys that are older than 3 hours
#-----------------------------------------------------------------------------------------------------------------------
SET GLOBAL event_scheduler = ON;
CREATE EVENT DeleteOrderKey
ON SCHEDULE EVERY 1 MINUTE
DO
    DELETE FROM OrderKey WHERE CreatedAt < (NOW() - INTERVAL 3 HOUR);