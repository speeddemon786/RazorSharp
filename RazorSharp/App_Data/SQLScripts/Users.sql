--Table: Users

/* DROP TABLE Users
GO */

CREATE TABLE Users (
  Id           int NOT NULL IDENTITY (1, 1),
  Username     nvarchar(100) NOT NULL,
  Password     nvarchar(250) NOT NULL,
  DisplayName  nvarchar(100),
  Email        nvarchar(250),
  IsActive     bit,
  DateCreated  datetime,
  /* Keys */
  CONSTRAINT PK__Users_Id
    PRIMARY KEY (Id)
)
GO

/*Add Admin User*/

BEGIN TRANSACTION
GO
INSERT INTO Users (Username, Password, DisplayName, Email, IsActive, DateCreated) VALUES (N'Admin', N'0W73ywDFn3VkAQKKLyaguaYbwqYhK/GaWalQH6Kzu9w=', N'Admin', N'admin@email.com', '1', CONVERT(datetime, '2016-01-29 13:16:08', 120));
COMMIT
GO