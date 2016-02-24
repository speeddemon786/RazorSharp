--Table: Menu

/* DROP TABLE Menu
GO */

CREATE TABLE Menu (
  Id         int NOT NULL IDENTITY (1, 1),
  mName      nvarchar(50),
  mURL       nvarchar(250),
  mOrderId   int NOT NULL DEFAULT 0,
  mTarget    nvarchar(10) NOT NULL DEFAULT '_self',
  mParentId  int NOT NULL DEFAULT 0,
  /* Keys */
  CONSTRAINT PK_Menu
    PRIMARY KEY (Id), 
  UNIQUE (Id)
)
GO