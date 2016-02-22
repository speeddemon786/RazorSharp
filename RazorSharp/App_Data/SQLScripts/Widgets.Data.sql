BEGIN TRANSACTION
GO
INSERT INTO Widgets (wName, wTitle, wText, wEditDate, wFile) VALUES (N'lipsum', N'Lipsum', '<p>Suspendisse potenti. Pellentesque dapibus egestas sem, vel rutrum velit elementum eget. Sed ullamcorper sodales dui, sed tincidunt orci. Proin euismod odio ut massa pharetra convallis. Ut a lorem malesuada, bibendum neque quis, lobortis magna. Donec vitae pharetra urna. Donec faucibus, erat id mollis lacinia, risus nulla cursus sem, et tempor eros dui non sapien. Maecenas id laoreet est, quis porttitor arcu. Morbi ac egestas lorem.</p>', CONVERT(datetime, '2015-10-01 11:36:31', 120), N'_Sidebar.cshtml');
INSERT INTO Widgets (wName, wTitle, wText, wEditDate, wFile) VALUES (N'title', N'Speed-Demon', '<p>Writings About Tech n Stuff...</p>', CONVERT(datetime, '2015-10-05 19:59:11', 120), N'_Slogan.cshtml');
INSERT INTO Widgets (wName, wTitle, wText, wEditDate, wFile) VALUES (N'categories', N'Categories', '<ul>
<li><a href="#">Aliquam libero</a></li>
<li><a href="#">Consectetuer adipiscing elit</a></li>
<li><a href="#">Metus aliquam pellentesque</a></li>
<li><a href="#">Suspendisse iaculis mauris</a></li>
<li><a href="#">Urnanet non molestie semper</a></li>
<li><a href="#">Proin gravida orci porttitor</a></li>
</ul>', CONVERT(datetime, '2015-10-05 19:34:46', 120), N'_Sidebar.cshtml');
INSERT INTO Widgets (wName, wTitle, wText, wEditDate, wFile) VALUES (N'contact-form', N'Contact Form', '', CONVERT(datetime, '2015-10-05 18:49:37', 120), N'_ContactForm.cshtml');
INSERT INTO Widgets (wName, wTitle, wText, wEditDate, wFile) VALUES (N'archives', N'Archives', '<ul>
<li><a href="#">Aliquam libero</a></li>
<li><a href="#">Consectetuer adipiscing elit</a></li>
<li><a href="#">Metus aliquam pellentesque</a></li>
<li><a href="#">Suspendisse iaculis mauris</a></li>
<li><a href="#">Urnanet non molestie semper</a></li>
<li><a href="#">Proin gravida orci porttitor</a></li>
</ul>', CONVERT(datetime, '2015-10-05 19:41:28', 120), N'_Sidebar.cshtml');
COMMIT
GO
