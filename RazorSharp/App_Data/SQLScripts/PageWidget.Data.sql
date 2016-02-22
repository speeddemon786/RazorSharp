BEGIN TRANSACTION
GO
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Top', 1, 40, 23);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 3, 39, 23);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Top', 1, 40, 29);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 3, 39, 29);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 3, 39, 30);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Top', 1, 40, 30);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Top', 1, 40, 31);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 3, 39, 31);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Bottom', 1, 42, 30);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 2, 43, 23);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 1, 44, 23);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 2, 43, 29);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 1, 44, 29);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 2, 43, 31);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 1, 44, 31);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 2, 43, 30);
INSERT INTO PageWidget (sName, wOrderId, wId, pId) VALUES (N'Right', 1, 44, 30);
COMMIT
GO
