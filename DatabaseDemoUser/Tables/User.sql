CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Email] NVARCHAR(250) NOT NULL UNIQUE,
    [Password] NVARCHAR(250) NOT NULL, 
    [Point] INT NOT NULL, 
    [IsActive] BIT NOT NULL
   


    
)
