CREATE TABLE Login (
  Username  nvarchar(50) NOT NULL,
  Password  nvarchar(50) NOT NULL,
  /* Keys */
  CONSTRAINT PK_Login
    PRIMARY KEY (Username), 
  UNIQUE (Username)
)
GO