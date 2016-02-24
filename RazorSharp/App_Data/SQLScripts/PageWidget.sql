--Table: PageWidget

/* DROP TABLE PageWidget
GO */

CREATE TABLE PageWidget (
  Id        int NOT NULL IDENTITY (1, 1),
  sName     nvarchar(100) NOT NULL,
  wOrderId  int NOT NULL DEFAULT 0,
  wId       int NOT NULL,
  pId       int,
  /* Keys */
  CONSTRAINT PK_PageWidget
    PRIMARY KEY (Id), 
  UNIQUE (Id),
  /* Foreign keys */
  CONSTRAINT DeletePage
    FOREIGN KEY (pId)
    REFERENCES Pages(pId)
    ON DELETE CASCADE
    ON UPDATE NO ACTION, 
  CONSTRAINT DeleteWidget
    FOREIGN KEY (wId)
    REFERENCES Widgets(Id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
)
GO