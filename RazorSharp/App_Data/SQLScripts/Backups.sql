--Table: Backups

/* DROP TABLE Backups
GO */

CREATE TABLE Backups (
  Id            int NOT NULL IDENTITY (1, 1),
  pwId          int NOT NULL,
  pwName        nvarchar(100) NOT NULL,
  pwTitle       nvarchar(250) NOT NULL,
  pwText        ntext NOT NULL,
  mTitle        nvarchar(100),
  mDescription  nvarchar(200),
  mKeywords     nvarchar(200),
  pMasterPage   nvarchar(50),
  pwDate        datetime NOT NULL,
  pBody         bit DEFAULT 'True',
  bType         nchar NOT NULL,
  pwFile        nvarchar(50) NOT NULL,
  /* Keys */
  PRIMARY KEY (Id), 
  UNIQUE (Id)
)
GO