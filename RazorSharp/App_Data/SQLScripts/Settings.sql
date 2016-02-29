--Table: Settings

/* DROP TABLE Settings
GO */

CREATE TABLE Settings (
  id      int NOT NULL IDENTITY (1, 1),
  Name    nvarchar(100) NOT NULL,
  Value   ntext,
  Source  nvarchar(100),
  /* Keys */
  CONSTRAINT PK__Settings_Id
    PRIMARY KEY (id)
)
GO