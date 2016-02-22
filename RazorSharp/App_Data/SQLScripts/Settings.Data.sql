BEGIN TRANSACTION
GO
INSERT INTO Settings (Name, Value, Source) VALUES (N'siteName', 'Speed-Demon', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'siteAnalytics', '<script>
  (function(i,s,o,g,r,a,m){i[''GoogleAnalyticsObject'']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,''script'',''//www.google-analytics.com/analytics.js'',''ga'');

  ga(''create'', ''UA-68711142-1'', ''auto'');
  ga(''send'', ''pageview'');

</script>', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'smtpServer', 'mail.speeddemon.co.za', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'smtpPort', '25', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'smtpSSL', 'False', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'smtpUser', 'grant@speeddemon.co.za', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'smtpPass', '@T3ch.B1Z', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'smtpFrom', 'grant@speeddemon.co.za', N'System');
INSERT INTO Settings (Name, Value, Source) VALUES (N'smtpTo', 'grant@speeddemon.co.za', N'System');
COMMIT
GO