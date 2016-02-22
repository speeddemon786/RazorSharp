BEGIN TRANSACTION
GO
INSERT INTO Menu (mName, mURL, mOrderId, mTarget, mParentId) VALUES (N'Home', N'~/', 4, N'_self', 0);
INSERT INTO Menu (mName, mURL, mOrderId, mTarget, mParentId) VALUES (N'About', N'~/About', 3, N'_self', 0);
INSERT INTO Menu (mName, mURL, mOrderId, mTarget, mParentId) VALUES (N'Contact', N'~/Contact', 1, N'_self', 0);
INSERT INTO Menu (mName, mURL, mOrderId, mTarget, mParentId) VALUES (N'Blog', N'~/Blog', 2, N'_self', 0);
COMMIT
GO
