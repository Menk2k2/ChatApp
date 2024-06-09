CREATE DATABASE QuanLyChat
GO

USE QuanLyChat
GO

--userAccount
--chanels
--chanel_user
--chanel_logs
--file

CREATE TABLE UserAccount
(
	id INT IDENTITY PRIMARY KEY, 
	DisplayName NVARCHAR(100) NOT NULL,
	UserName NVARCHAR(100) NOT NULL,
	PassWord NVARCHAR(1000) NOT NULL,
	Department NVARCHAR(100),
	Type INT NOT NULL
)
GO

CREATE TABLE Channels
(
    id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    LastComment DATETIME,
    NumberOfUsers INT NOT NULL
)
GO

CREATE TABLE Channel_User
(
    id INT IDENTITY PRIMARY KEY,
    UserID INT NOT NULL,
    ChannelID INT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES UserAccount(ID),
    FOREIGN KEY (ChannelID) REFERENCES Channels(ID)
)
GO

CREATE TABLE Files
(
    id INT IDENTITY PRIMARY KEY,
    Path NVARCHAR(MAX)
)
GO

CREATE TABLE Channel_Logs
(
    id INT IDENTITY PRIMARY KEY,
    ChannelID INT NOT NULL,
    UserID INT NOT NULL,
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME NOT NULL,
    FileID INT,
    FOREIGN KEY (ChannelID) REFERENCES Channels(ID),
    FOREIGN KEY (UserID) REFERENCES UserAccount(ID),
    FOREIGN KEY (FileID) REFERENCES Files(ID)
)
GO


