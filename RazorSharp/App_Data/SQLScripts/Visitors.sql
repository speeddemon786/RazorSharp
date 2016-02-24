--Table: Visitors

/* DROP TABLE Visitors
GO */

CREATE TABLE Visitors (
  Id               int NOT NULL IDENTITY (1, 1),
  [TimeStamp]      datetime,
  Page             nvarchar(250),
  Browser          nvarchar(50),
  BrowserVersion   nvarchar(50),
  OperatingSystem  nvarchar(50),
  IPAddress        nvarchar(50),
  DNSName          nvarchar(100),
  Country          nvarchar(50),
  Referer          nvarchar(250),
  /* Keys */
  PRIMARY KEY (Id), 
  UNIQUE (Id)
)
GO