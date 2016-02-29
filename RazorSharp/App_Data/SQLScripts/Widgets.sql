--Table: Widgets

/* DROP TABLE Widgets
GO */

CREATE TABLE Widgets (
  Id         int NOT NULL IDENTITY (1, 1),
  wName      nvarchar(100) NOT NULL,
  wTitle     nvarchar(100) NOT NULL,
  wText      ntext NOT NULL,
  wEditDate  datetime NOT NULL,
  wFile      nvarchar(100) NOT NULL,
  /* Keys */
  CONSTRAINT PK_Widgets_Id
    PRIMARY KEY (Id), 
  CONSTRAINT UQ__Widgets__Id
    UNIQUE (Id), 
  CONSTRAINT UQ__Widgets__wName
    UNIQUE (wName)
)
GO