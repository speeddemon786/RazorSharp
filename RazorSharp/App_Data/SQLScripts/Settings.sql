CREATE TABLE Settings (
  id      int NOT NULL IDENTITY (1, 1),
  Name    nvarchar(100) NOT NULL,
  Value   ntext,
  Source  nvarchar(100),
  /* Keys */
  PRIMARY KEY (id)
)
GO