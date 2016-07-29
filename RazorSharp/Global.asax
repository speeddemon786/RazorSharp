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

    void Application_Error(object sender, EventArgs e)
    {
        var exception = Server.GetLastError();
        Response.Clear();
        if (!HttpContext.Current.IsCustomErrorEnabled)
            return;

        //Comment out line below to disbale logging
        ExceptionUtility.LogException(exception, "Global.asax");

        //Comment out line below to not email admin on error
        ExceptionUtility.NotifyAdmin(exception, "Global.asax");

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
        Server.ClearError();
        Response.TrySkipIisCustomErrors = true;
        Response.Redirect(string.Format("~/Error?Error={0}", action));
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
    }

</script>
