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
  pAuthor       nvarchar(100),
  pBody         bit NOT NULL DEFAULT 'True',
  pHTML         bit NOT NULL DEFAULT 'True',
  pBodyFile     nvarchar(50) NOT NULL,
  /* Keys */
  CONSTRAINT PK__Pages_Id
    PRIMARY KEY (pId), 
  CONSTRAINT UQ__Pages__pId
    UNIQUE (pId), 
  CONSTRAINT UQ__Pages_pName
    UNIQUE (pName)
)
GO