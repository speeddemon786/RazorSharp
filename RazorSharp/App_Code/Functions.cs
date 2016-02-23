using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;
using System.Web.WebPages;
using System.Web.WebPages.Html;
using WebMatrix.Data;

/// <summary>
/// Helper Functions
/// </summary>

public static class Functions
{
    /// sql compact db name
    public static string GetDBName()
    {
        return ConfigurationManager.AppSettings["DBName"];
    }

    /// clean names - remove _, .cshtml from names
    public static string CleanNames(string name)
    {
        name = name.Replace("_", "");
        name = name.Replace(".cshtml", "");
        return name;
    }

    ///return shortened string value
    public static string GetShortString(string strlong, int length)
    {
        if (strlong.Length <= length)
        {
            return strlong;
        }
        else
        {
            return strlong.Substring(0, length) + "...";
        }
    }

    /// lists all the body pages in /Templates/Body and in /Addon/addonName/Body folder
    public static List<SelectListItem> pageBodies(string pBodyFile)
    {
        var pBodies = new List<SelectListItem>();
        // list all the body pages
        HttpContext Context = HttpContext.Current;
        DirectoryInfo dirInfoBody = new DirectoryInfo(Context.Server.MapPath("~/Templates/Body"));
        FileInfo[] fileInfoBody = dirInfoBody.GetFiles("_*.cshtml", SearchOption.TopDirectoryOnly);
        foreach (var f in fileInfoBody)
        {
            pBodies.Add(new SelectListItem { Text = CleanNames(f.Name), Value = f.Name, Selected = pBodyFile == f.Name ? true : false });
        }
        //List all  pBody files in addons
        DirectoryInfo dirInfoBody1 = new DirectoryInfo(Context.Server.MapPath("~/Addons"));
        DirectoryInfo[] listAddons = dirInfoBody1.GetDirectories("*", SearchOption.TopDirectoryOnly);
        foreach (var d in listAddons)
        {
            FileInfo[] nameAddons = dirInfoBody1.GetFiles(d + "/Body/_*.cshtml", SearchOption.TopDirectoryOnly);

            foreach (var n in nameAddons)
            {
                pBodies.Add(new SelectListItem { Text = CleanNames(n.Name), Value = n.Name, Selected = pBodyFile == n.Name ? true : false });
            }
        }
        return pBodies;
    }

    /// lists all the page layouts in /Layouts
    public static List<SelectListItem> pageLayouts(string pMasterPage)
    {
        var pLayouts = new List<SelectListItem>();
        HttpContext Context = HttpContext.Current;
        DirectoryInfo dirInfo = new DirectoryInfo(Context.Server.MapPath("~/Layouts"));
        FileInfo[] fileInfo = dirInfo.GetFiles("_*.cshtml", SearchOption.TopDirectoryOnly);
        foreach (var f in fileInfo)
        {
            pLayouts.Add(new SelectListItem { Text = CleanNames(f.Name), Value = f.Name, Selected = pMasterPage == f.Name ? true : false });
        }
        return pLayouts;
    }

    /// lists all the widget layout templates in /Templates
    public static List<SelectListItem> widgetLayouts(string wFile)
    {
        var wLayouts = new List<SelectListItem>();
        HttpContext Context = HttpContext.Current;
        DirectoryInfo dirInfo = new DirectoryInfo(Context.Server.MapPath("~/Templates"));
        FileInfo[] fileInfo = dirInfo.GetFiles("_*.cshtml", SearchOption.TopDirectoryOnly);
        foreach (var f in fileInfo)
        {
            wLayouts.Add(new SelectListItem { Text = CleanNames(f.Name), Value = f.Name, Selected = wFile == f.Name ? true : false });
        }
        return wLayouts;
    }

    /// list all pages in db
    public static List<SelectListItem> pageNames(string mPage)
    {
        var pNames = new List<SelectListItem>();
        IEnumerable<dynamic> pages = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select pName From Pages Where pHTML=1 Order By pName";
            pages = Db.Query(SqlSelect);
        }
        pNames.Add(new SelectListItem { Text = "Select One", Value = "" });
        foreach (var row in pages)
        {
            pNames.Add(new SelectListItem { Text = row.pName, Value = row.pName, Selected = mPage == row.pName ? true : false });
        }
        return pNames;
    }

    /// list page target type
    public static List<SelectListItem> pageTarget(string mTarget)
    {
        var pTargets = new List<SelectListItem>();
        pTargets.Add(new SelectListItem { Text = "Same window", Value = "_self", Selected = mTarget == "_self" ? true : false });
        pTargets.Add(new SelectListItem { Text = "New window", Value = "_blank", Selected = mTarget == "_blank" ? true : false });
        return pTargets;
    }

    /// smtp ssl options
    public static List<SelectListItem> emailSSL(bool smtpSSL)
    {
        var eSSL = new List<SelectListItem>();
        eSSL.Add(new SelectListItem { Text = "Yes", Value = "true", Selected = smtpSSL == true ? true : false });
        eSSL.Add(new SelectListItem { Text = "No", Value = "false", Selected = smtpSSL == false ? true : false });
        return eSSL;
    }

    /// list page parents
    public static List<SelectListItem> pageParent(int mParentId)
    {
        var pParents = new List<SelectListItem>();
        IEnumerable<dynamic> Parents = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select * From Menu Order By mOrderId Desc";
            Parents = Db.Query(SqlSelect);
        }
        var parentItems = Parents.Where(d => d.mParentId == 0);
        pParents.Add(new SelectListItem { Text = "Top Menu Item", Value = "0", Selected = mParentId == 0 ? true : false });
        foreach (var row in parentItems)
        {
            pParents.Add(new SelectListItem { Text = row.mName, Value = row.Id.ToString(), Selected = mParentId == row.Id ? true : false });
        }
        return pParents;
    }

    /// list widget zones on layout page
    public static List<SelectListItem> widgetZones(string pageId, string sName)
    {
        var wZones = new List<SelectListItem>();
        // read layout page and find all available zones
        dynamic masterPage = null;
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select pMasterPage From Pages Where pId=@0";
            masterPage = Db.QueryValue(SqlSelect, pageId);
        }
        //read master and find zones
        HttpContext Context = HttpContext.Current;
        StreamReader textFile = new StreamReader(Context.Server.MapPath("~/Layouts/" + masterPage));
        string fileContents = textFile.ReadToEnd();
        textFile.Close();
        System.Collections.ArrayList availZones = new System.Collections.ArrayList();
        string pattern = "@RenderSection\\(\"[\\w]*";
        foreach (Match m in Regex.Matches(fileContents, pattern))
        {
            availZones.Add(m.ToString().Replace("@RenderSection(\"", ""));
        }
        //should list only unique zones, ex.: if zones are inside IF will create duplicates, so :
        object[] arr = availZones.ToArray();
        var distZones = arr.Distinct();
        foreach (var zone in distZones)
        {
            wZones.Add(new SelectListItem { Text = CleanNames(zone.ToString()), Value = zone.ToString(), Selected = sName == zone.ToString() ? true : false });
        }
        return wZones;
    }

    /// list widgets in db
    public static List<SelectListItem> availableWidgets(string wId)
    {
        var lstWidgets = new List<SelectListItem>();
        IEnumerable<dynamic> Widgets = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(GetDBName()))
        {
            var sqlSelectWidgets = "Select Id, wName From Widgets Order By wName";
            Widgets = Db.Query(sqlSelectWidgets);
        }
        foreach (var row in Widgets)
        {
            lstWidgets.Add(new SelectListItem { Text = row.wName, Value = row.Id.ToString(), Selected = wId.AsInt() == row.Id ? true : false });
        }
        return lstWidgets;
    }

    // list page backups in db
    public static List<SelectListItem> pageBackups(string pId)
    {
        var pBackups = new List<SelectListItem>();
        IEnumerable<dynamic> Backups = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select Id, pwDate From Backups Where bType='p' And pwID=@0 Order By id Desc";
            Backups = Db.Query(SqlSelect, pId);
        }
        pBackups.Add(new SelectListItem { Text = "Load From Backup", Value = "-1" });
        foreach (var f in Backups.Take(5))
        {
            pBackups.Add(new SelectListItem { Text = f.pwDate.ToString(), Value = f.Id.ToString() });
        }
        pBackups.Add(new SelectListItem { Text = "Load Current Version", Value = "0" });
        return pBackups;
    }

    // list widget backups in db
    public static List<SelectListItem> widgetBackups(string Id)
    {
        var wBackups = new List<SelectListItem>();
        IEnumerable<dynamic> Backups = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select Id, pwDate From Backups where bType='w' And pwID=@0 Order By id Desc";
            Backups = Db.Query(SqlSelect, Id);
        }
        wBackups.Add(new SelectListItem { Text = "Load From Backup", Value = "-1" });
        foreach (var f in Backups.Take(5))
        {
            wBackups.Add(new SelectListItem { Text = f.pwDate.ToString(), Value = f.Id.ToString() });
        }
        wBackups.Add(new SelectListItem { Text = "Load Current Version", Value = "0" });
        return wBackups;
    }

    //
    public static List<string> menuItems()
    {
        var listpPages = new List<string>();
        IEnumerable<dynamic> pages = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select pName From Pages Where pHTML=1 Order By pName";
            pages = Db.Query(SqlSelect);
        }
        foreach (var row in pages)
        {
            listpPages.Add(row.pName);
        }
        return listpPages;
    }

    // put spaces in addons names
    public static string addonsMenu(string name)
    {
        string _retV;
        _retV = string.Concat(name.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        return _retV;
    }

    // empty directory
    public static void emptyDirectory(this DirectoryInfo directory)
    {
        foreach(FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }
        foreach(DirectoryInfo subDirectory in directory.GetDirectories())
        {
            subDirectory.Delete(true);
        }
    }

    // Gets userid based on username
    public static string getUserID(string username)
    {
        string userId = string.Empty;
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select Id From Users Where Username = @0";
            userId = Db.QueryValue(SqlSelect, username).ToString();
        }
        return userId;
    }

    // validate is user exists
    public static bool userExists(string username)
    {
        bool exists = false;
        using (var Db = Database.Open(GetDBName()))
        {
            var SqlSelect = "Select Count(*) From Users Where Username = @0";
            int count = (int)Db.QueryValue(SqlSelect, username);
            if (count > 0)
            {
                exists = true;
            }
        }
        return exists;
    }
}