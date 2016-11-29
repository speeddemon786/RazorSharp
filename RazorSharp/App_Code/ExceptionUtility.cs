using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Helpers;

/// <summary>
/// class for logging errors and sending email notifications to admin
/// </summary>

public sealed class ExceptionUtility
{
    public static void LogException(Exception ex, string source)
    {
        var logFile = "~/App_Data/ErrorLog.txt";
        logFile = HttpContext.Current.Server.MapPath(logFile);

        var sw = new StreamWriter(logFile, true);
        sw.WriteLine("********** {0} **********", DateTime.Now);
        if (ex.InnerException != null)
        {
            sw.Write("Inner Exception Type: ");
            sw.WriteLine(ex.InnerException.GetType().ToString());
            sw.Write("Inner Exception: ");
            sw.WriteLine(ex.InnerException.Message);
            sw.Write("Inner Source: ");
            sw.WriteLine(ex.InnerException.Source);
            if (ex.InnerException.StackTrace != null)
            {
                sw.WriteLine("Inner Stack Trace: ");
                sw.WriteLine(ex.InnerException.StackTrace);
            }
        }
        sw.Write("Exception Type: ");
        sw.WriteLine(ex.GetType().ToString());
        sw.WriteLine("Exception: " + ex.Message);
        sw.WriteLine("Source: " + source);
        sw.WriteLine("Stack Trace: ");
        if (ex.StackTrace != null)
        {
            sw.WriteLine(ex.StackTrace);
            sw.WriteLine();
        }
        sw.Close();
    }

    public static void NotifyAdmin(Exception ex, string source)
    {
        var emailTo = ConfigurationManager.AppSettings["EmailTo"];
        var sb = new StringBuilder();
        sb.Append("********** " + DateTime.Now + " **********");
        sb.AppendLine();
        if (ex.InnerException != null)
        {
            sb.Append("Inner Exception Type: ");
            sb.Append(ex.InnerException.GetType());
            sb.AppendLine();
            sb.Append("Inner Exception: ");
            sb.Append(ex.InnerException.Message);
            sb.AppendLine();
            sb.Append("Inner Source: ");
            sb.Append(ex.InnerException.Source);
            sb.AppendLine();
            if (ex.InnerException.StackTrace != null)
            {
                sb.Append("Inner Stack Trace: ");
                sb.Append(ex.InnerException.StackTrace);
                sb.AppendLine();
            }
        }
        sb.Append("Exception Type: ");
        sb.Append(ex.GetType());
        sb.AppendLine();
        sb.Append("Exception: " + ex.Message);
        sb.AppendLine();
        sb.Append("Source: " + source);
        sb.AppendLine();
        sb.Append("Stack Trace: ");
        if (ex.StackTrace != null)
        {
            sb.Append(ex.StackTrace);
            sb.AppendLine();
        }
        var mailBody = sb.ToString();
        WebMail.SmtpServer = ConfigurationManager.AppSettings["SMTPServer"];
        WebMail.SmtpPort = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]);
        WebMail.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["SMTPSSL"]);
        WebMail.UserName = ConfigurationManager.AppSettings["SMTPUser"];
        WebMail.Password = ConfigurationManager.AppSettings["SMTPPass"];
        WebMail.From = ConfigurationManager.AppSettings["SMTPUser"];
        WebMail.Send(emailTo, "Website : An Error Occured!", mailBody, isBodyHtml: false);
    }
}