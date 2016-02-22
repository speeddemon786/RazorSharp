<%@ Application Language="C#" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup

    }

    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown
    }

    protected void Application_Error()
    {
        // Code that runs when an unhandled error occurs
        var exception = Server.GetLastError();
        Response.Clear();

        //customErrors=On/RemoteOnly+nonLocal
        if (!HttpContext.Current.IsCustomErrorEnabled)
            return; //wants to see YSOD

        //log it
        ExceptionUtility.LogException(exception, "Global.asax");

        //notify admin
        ExceptionUtility.NotifyAdmin(exception, "Global.asax");

        //is this a specific error?
        var httpException = exception as HttpException;
        string action = null;
        if (httpException != null)
        {
            var code = httpException.GetHttpCode();
            if (code == 400)
                action = "BadRequest";
            if (code == 401)
                action = "Unauthorized";
            if (code == 403)
                action = "Forbidden";
            if (code == 404)
                action = "NotFound";
            if (code == 500)
                action = "InternalServerError";
        }
        Server.ClearError(); //make sure customErrors doesn't take over
        Response.TrySkipIisCustomErrors = true; //don't let IIS7 take over
        Response.Redirect(String.Format("~/Error?Error={0}", action));
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }

</script>
