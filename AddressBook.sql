CREATE DATABASE AddressBook
use AddressBook
CREATE TABLE [dbo].[Country] (
    [CountryID] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    [CountryName] VARCHAR(100) UNIQUE NOT NULL,
    [CountryCode] VARCHAR(50) NOT NULL,
    [CreationDate] DATETIME DEFAULT GETDATE(),
    [UserID] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[User]([UserID])
);
CREATE TABLE [dbo].[State] (
    [StateID] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    [CountryID] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Country]([CountryID]),
    [StateName] VARCHAR(100) UNIQUE NOT NULL,
    [StateCode] VARCHAR(50) NOT NULL,
    [CreationDate] DATETIME DEFAULT GETDATE(),
    [UserID] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[User]([UserID])
);
CREATE TABLE [dbo].[City] (
    [CityID] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    [StateID] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[State]([StateID]),
    [CountryID] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Country]([CountryID]),
    [CityName] VARCHAR(100) UNIQUE NOT NULL,
    [STDCode] VARCHAR(50) NULL,
    [PinCode] VARCHAR(6) NULL,
    [CreationDate] DATETIME DEFAULT GETDATE(),
    [UserID] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[User]([UserID])
);
CREATE TABLE [dbo].[User] (
    [UserID] INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    [UserName] VARCHAR(100) NOT NULL,
    [MobileNo] VARCHAR(50) NOT NULL,
    [EmailID] VARCHAR(100) NOT NULL,
    [CreationDate] DATETIME DEFAULT GETDATE()
);
--SP
--Country
CREATE OR ALTER PROCEDURE [dbo].[PR_Country_SelectAll]
AS
BEGIN
    SELECT 
        c.[CountryID], c.[CountryName], c.[CountryCode], c.[CreationDate], c.[UserID], 
        u.[UserName]
    FROM [dbo].[Country] c
    INNER JOIN [dbo].[User] u ON c.[UserID] = u.[UserID]
    ORDER BY c.[CountryName];
END;

CREATE OR ALTER PROCEDURE [dbo].[PR_Country_SelectByPK]
    @CountryID INT
AS
BEGIN
    SELECT 
        c.[CountryID], c.[CountryName], c.[CountryCode], c.[CreationDate], c.[UserID], 
        u.[UserName]
    FROM [dbo].[Country] c
    INNER JOIN [dbo].[User] u ON c.[UserID] = u.[UserID]
    WHERE c.[CountryID] = @CountryID;
END;

CREATE OR ALTER PROCEDURE [dbo].[PR_Country_Insert]
    @CountryName VARCHAR(100),
    @CountryCode VARCHAR(50),
    @UserID INT
AS
BEGIN
    INSERT INTO [dbo].[Country] ([CountryName], [CountryCode], [UserID])
    VALUES (@CountryName, @CountryCode, @UserID);
END;
SELECT * FROM COUNTRY
CREATE OR ALTER PROCEDURE [dbo].[PR_Country_UpdateByPK]
    @CountryID INT,
    @CountryName VARCHAR(100),
    @CountryCode VARCHAR(50),
    @UserID INT
AS
BEGIN
    UPDATE [dbo].[Country]
    SET [CountryName] = @CountryName,
        [CountryCode] = @CountryCode,
        [UserID] = @UserID
    WHERE [CountryID] = @CountryID;
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_Country_DeleteByPK]
    @CountryID INT
AS
BEGIN
    DELETE FROM [dbo].[Country]
    WHERE [CountryID] = @CountryID;
END;
--STATE
CREATE OR ALTER PROCEDURE [dbo].[PR_State_SelectAll]
AS
BEGIN
    SELECT 
        s.[StateID], s.[StateName], s.[StateCode], s.[CreationDate], s.[UserID], 
        c.[CountryName], u.[UserName]
    FROM [dbo].[State] s
    INNER JOIN [dbo].[Country] c ON s.[CountryID] = c.[CountryID]
    INNER JOIN [dbo].[User] u ON s.[UserID] = u.[UserID]
    ORDER BY s.[StateName];
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_State_SelectByPK]
    @StateID INT
AS
BEGIN
    SELECT 
        s.[StateID], s.[StateName], s.[StateCode], s.[CreationDate], s.[UserID], 
        c.[CountryName], u.[UserName]
    FROM [dbo].[State] s
    INNER JOIN [dbo].[Country] c ON s.[CountryID] = c.[CountryID]
    INNER JOIN [dbo].[User] u ON s.[UserID] = u.[UserID]
    WHERE s.[StateID] = @StateID;
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_State_Insert]
    @CountryID INT,
    @StateName VARCHAR(100),
    @StateCode VARCHAR(50),
    @UserID INT
AS
BEGIN
    INSERT INTO [dbo].[State] ([CountryID], [StateName], [StateCode], [UserID])
    VALUES (@CountryID, @StateName, @StateCode, @UserID);
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_State_UpdateByPK]
    @StateID INT,
    @CountryID INT,
    @StateName VARCHAR(100),
    @StateCode VARCHAR(50),
    @UserID INT
AS
BEGIN
    UPDATE [dbo].[State]
    SET [CountryID] = @CountryID,
        [StateName] = @StateName,
        [StateCode] = @StateCode,
        [UserID] = @UserID
    WHERE [StateID] = @StateID;
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_State_DeleteByPK]
    @StateID INT
AS
BEGIN
    DELETE FROM [dbo].[State]
    WHERE [StateID] = @StateID;
END;
--CITY
CREATE OR ALTER PROCEDURE [dbo].[PR_City_SelectAll]
AS
BEGIN
    SELECT 
        ci.[CityID], ci.[CityName], ci.[STDCode], ci.[PinCode], ci.[CreationDate], ci.[UserID], 
        s.[StateName], c.[CountryName], u.[UserName]
    FROM [dbo].[City] ci
    INNER JOIN [dbo].[State] s ON ci.[StateID] = s.[StateID]
    INNER JOIN [dbo].[Country] c ON ci.[CountryID] = c.[CountryID]
    INNER JOIN [dbo].[User] u ON ci.[UserID] = u.[UserID]
    ORDER BY ci.[CityName];
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_City_SelectByPK]
    @CityID INT
AS
BEGIN
    SELECT 
        ci.[CityID], ci.[CityName], ci.[STDCode], ci.[PinCode], ci.[CreationDate], ci.[UserID], 
        s.[StateName], c.[CountryName], u.[UserName]
    FROM [dbo].[City] ci
    INNER JOIN [dbo].[State] s ON ci.[StateID] = s.[StateID]
    INNER JOIN [dbo].[Country] c ON ci.[CountryID] = c.[CountryID]
    INNER JOIN [dbo].[User] u ON ci.[UserID] = u.[UserID]
    WHERE ci.[CityID] = @CityID;
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_City_Insert]
    @StateID INT,
    @CountryID INT,
    @CityName VARCHAR(100),
    @STDCode VARCHAR(50),
    @PinCode VARCHAR(6),
    @UserID INT
AS
BEGIN
    INSERT INTO [dbo].[City] ([StateID], [CountryID], [CityName], [STDCode], [PinCode], [UserID])
    VALUES (@StateID, @CountryID, @CityName, @STDCode, @PinCode, @UserID);
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_City_UpdateByPK]
    @CityID INT,
    @StateID INT,
    @CountryID INT,
    @CityName VARCHAR(100),
    @STDCode VARCHAR(50),
    @PinCode VARCHAR(6),
    @UserID INT
AS
BEGIN
    UPDATE [dbo].[City]
    SET [StateID] = @StateID,
        [CountryID] = @CountryID,
        [CityName] = @CityName,
        [STDCode] = @STDCode,
        [PinCode] = @PinCode,
        [UserID] = @UserID
    WHERE [CityID] = @CityID;
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_City_DeleteByPK]
    @CityID INT
AS
BEGIN
    DELETE FROM [dbo].[City]
    WHERE [CityID] = @CityID;
END;
--USER
CREATE OR ALTER PROCEDURE [dbo].[PR_User_SelectAll]
AS
BEGIN
    SELECT 
        u.[UserID], u.[UserName], u.[MobileNo], u.[EmailID], u.[CreationDate]
    FROM [dbo].[User] u
    ORDER BY u.[UserName];
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_User_SelectByPK]
    @UserID INT
AS
BEGIN
    SELECT 
        u.[UserID], u.[UserName], u.[MobileNo], u.[EmailID], u.[CreationDate]
    FROM [dbo].[User] u
    WHERE u.[UserID] = @UserID;
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_User_Insert]
    @UserName VARCHAR(100),
    @MobileNo VARCHAR(50),
    @EmailID VARCHAR(100)
AS
BEGIN
    INSERT INTO [dbo].[User] ([UserName], [MobileNo], [EmailID])
    VALUES (@UserName, @MobileNo, @EmailID);
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_User_UpdateByPK]
    @UserID INT,
    @UserName VARCHAR(100),
    @MobileNo VARCHAR(50),
    @EmailID VARCHAR(100)
AS
BEGIN
    UPDATE [dbo].[User]
    SET [UserName] = @UserName,
        [MobileNo] = @MobileNo,
        [EmailID] = @EmailID
    WHERE [UserID] = @UserID;
END;
CREATE OR ALTER PROCEDURE [dbo].[PR_User_DeleteByPK]
    @UserID INT
AS
BEGIN
    DELETE FROM [dbo].[User]
    WHERE [UserID] = @UserID;
END;
CREATE OR ALTER PROC [dbo].[PR_User_ValidateLogin]
@UserName VARCHAR(20),
@MobileNo Varchar(20),
@EmailID VARCHAR(20)
AS
BEGIN
	SELECT [dbo].[User].[UserID],[dbo].[User].[UserName],[dbo].[User].[MobileNo],[dbo].[User].[EmailID] FROM [dbo].[User]
	WHERE [dbo].[User].[UserName]=@UserName AND [dbo].[User].[MobileNo]=@MobileNo AND [dbo].[User].[EmailID]=@EmailID
END
--Insert Data
INSERT INTO [dbo].[User] (UserName, MobileNo, EmailID, CreationDate)
VALUES 
('Rahul Sharma', '9876543210', 'rahul.sharma@gmail.com', GETDATE()),
('Priya Agarwal', '9823456789', 'priya.agarwal@yahoo.com', GETDATE()),
('Arjun Verma', '9898123456', 'arjun.verma@outlook.com', GETDATE()),
('Neha Patel', '9876509876', 'neha.patel@rediffmail.com', GETDATE()),
('Vishal Kumar', '9812345678', 'vishal.kumar@indiatimes.com', GETDATE()),
('Sneha Nair', '9934567890', 'sneha.nair@hotmail.com', GETDATE()),
('Ananya Mishra', '9845098765', 'ananya.mishra@gmail.com', GETDATE()),
('Karan Singh', '9876123450', 'karan.singh@yahoo.com', GETDATE()),
('Ravi Chopra', '9812765432', 'ravi.chopra@outlook.com', GETDATE()),
('Meera Iyer', '9898989898', 'meera.iyer@rediffmail.com', GETDATE());




INSERT INTO [dbo].[Country] (CountryName, CountryCode, UserID, CreationDate)
VALUES 
('India', 'IN', 1, GETDATE()),
('United States', 'US', 2, GETDATE()),
('United Kingdom', 'UK', 3, GETDATE()),
('Canada', 'CA', 4, GETDATE()),
('Australia', 'AU', 5, GETDATE());




INSERT INTO [dbo].[State] (CountryID, StateName, StateCode, UserID, CreationDate)
VALUES 
(1, 'Gujarat', 'GJ', 1, GETDATE()),
(1, 'Maharashtra', 'MH', 2, GETDATE()),
(1, 'Karnataka', 'KA', 3, GETDATE()),
(1, 'Tamil Nadu', 'TN', 4, GETDATE()),
(1, 'Uttar Pradesh', 'UP', 5, GETDATE()),
(1, 'Rajasthan', 'RJ', 6, GETDATE()),
(1, 'West Bengal', 'WB', 7, GETDATE()),
(1, 'Madhya Pradesh', 'MP', 8, GETDATE()),
(1, 'Bihar', 'BR', 9, GETDATE()),
(1, 'Punjab', 'PB', 10, GETDATE());




INSERT INTO [dbo].[City] (StateID, CountryID, CityName, STDCode, PinCode, UserID, CreationDate)
VALUES 
(1, 1, 'Ahmedabad', '079', '380001', 1, GETDATE()),
(2, 1, 'Mumbai', '022', '400001', 2, GETDATE()),
(3, 1, 'Bengaluru', '080', '560001', 3, GETDATE()),
(4, 1, 'Chennai', '044', '600001', 4, GETDATE()),
(5, 1, 'Lucknow', '0522', '226001', 5, GETDATE()),
(6, 1, 'Jaipur', '0141', '302001', 6, GETDATE()),
(7, 1, 'Kolkata', '033', '700001', 7, GETDATE()),
(8, 1, 'Bhopal', '0755', '462001', 8, GETDATE()),
(9, 1, 'Patna', '0612', '800001', 9, GETDATE()),
(10, 1, 'Chandigarh', '0172', '160001', 10, GETDATE());