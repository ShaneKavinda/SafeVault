CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(100),
    Email VARCHAR(100)
);

ALTER TABLE Users
ADD Password VARCHAR(255) NOT NULL;