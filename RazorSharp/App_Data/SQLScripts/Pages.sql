--Table: Pages

/* DROP TABLE Pages
GO */

CREATE TABLE Pages (
  pId           int NOT NULL IDENTITY (1, 1),
  pName         nvarchar(100) NOT NULL,
  pTitle        nvarchar(250) NOT NULL,
  pText         ntext NOT NULL,
  mTitle        nvarchar(100) NOT NULL,
  mDescription  nvarchar(200) NOT NULL,
  mKeywords     nvarchar(200) NOT NULL,
  pMasterPage   nvarchar(50) NOT NULL,
  pEditDate     datetime NOT NULL,
  pBody         bit NOT NULL DEFAULT 'True',
  pHTML         bit NOT NULL DEFAULT 'True',
  pBodyFile     nvarchar(50) NOT NULL,
  /* Keys */
  PRIMARY KEY (pId), 
  UNIQUE (pId), 
  UNIQUE (pName)
)
GO

CREATE INDEX PK__Pages__00000000000000AC
  ON Pages
  (pId)
GO