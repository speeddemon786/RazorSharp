using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.WebPages;
using System.Web.WebPages.Html;
using WebMatrix.Data;

/// <summary>
/// Helper Functions
/// </summary>

public static class Functions
{
    public static string GetDbName()
    {
        return ConfigurationManager.AppSettings["DBName"];
    }

    public static string CleanNames(string name)
    {
        name = name.Replace("_", "");
        name = name.Replace(".cshtml", "");
        return name;
    }

    public static string GetShortString(string strlong, int length)
    {
        if (strlong.Length <= length)
        {
            return strlong;
        }
        return strlong.Substring(0, length) + "...";
    }

    public static List<SelectListItem> PageBodies(string pBodyFile)
    {
        var context = HttpContext.Current;
        var dirInfoBody = new DirectoryInfo(context.Server.MapPath("~/Templates/Body"));
        var fileInfoBody = dirInfoBody.GetFiles("_*.cshtml", SearchOption.TopDirectoryOnly);
        var pBodies = fileInfoBody.Select(f => new SelectListItem {Text = CleanNames(f.Name), Value = f.Name, Selected = pBodyFile == f.Name}).ToList();
        var dirInfoBody1 = new DirectoryInfo(context.Server.MapPath("~/Addons"));
        var listAddons = dirInfoBody1.GetDirectories("*", SearchOption.TopDirectoryOnly);
        pBodies.AddRange(from d in listAddons from n in dirInfoBody1.GetFiles(d + "/Body/_*.cshtml", SearchOption.TopDirectoryOnly) select new SelectListItem {Text = CleanNames(n.Name), Value = n.Name, Selected = pBodyFile == n.Name});
        return pBodies;
    }

    public static List<SelectListItem> PageLayouts(string pMasterPage)
    {
        var context = HttpContext.Current;
        var dirInfo = new DirectoryInfo(context.Server.MapPath("~/Layouts"));
        var fileInfo = dirInfo.GetFiles("_*.cshtml", SearchOption.TopDirectoryOnly);
        return fileInfo.Select(f => new SelectListItem {Text = CleanNames(f.Name), Value = f.Name, Selected = pMasterPage == f.Name}).ToList();
    }

    public static List<SelectListItem> WidgetLayouts(string wFile)
    {
        var context = HttpContext.Current;
        var dirInfo = new DirectoryInfo(context.Server.MapPath("~/Templates"));
        var fileInfo = dirInfo.GetFiles("_*.cshtml", SearchOption.TopDirectoryOnly);
        return fileInfo.Select(f => new SelectListItem {Text = CleanNames(f.Name), Value = f.Name, Selected = wFile == f.Name}).ToList();
    }

    public static List<SelectListItem> PageNames(string mPage)
    {
        var pNames = new List<SelectListItem>();
        IEnumerable<dynamic> pages;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select pName From Pages Where pHTML=1 Order By pName";
            pages = db.Query(sqlSelect);
        }
        pNames.Add(new SelectListItem { Text = "Select One", Value = "" });
        pNames.AddRange(pages.Select(row => new SelectListItem {Text = row.pName, Value = row.pName, Selected = mPage == row.pName}));
        return pNames;
    }

    public static List<SelectListItem> PageTarget(string mTarget)
    {
        var pTargets = new List<SelectListItem>
        {
            new SelectListItem {Text = "Same window", Value = "_self", Selected = mTarget == "_self"},
            new SelectListItem {Text = "New window", Value = "_blank", Selected = mTarget == "_blank"}
        };
        return pTargets;
    }

    public static List<SelectListItem> EmailSsl(bool smtpSsl)
    {
        var eSsl = new List<SelectListItem>
        {
            new SelectListItem {Text = "Yes", Value = "true", Selected = smtpSsl},
            new SelectListItem {Text = "No", Value = "false", Selected = smtpSsl == false}
        };
        return eSsl;
    }

    public static List<SelectListItem> PageParent(int mParentId)
    {
        var pParents = new List<SelectListItem>();
        IEnumerable<dynamic> parents;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select * From Menu Order By mOrderId Desc";
            parents = db.Query(sqlSelect);
        }
        var parentItems = parents.Where(d => d.mParentId == 0);
        pParents.Add(new SelectListItem { Text = "Top Menu Item", Value = "0", Selected = mParentId == 0 });
        pParents.AddRange(parentItems.Select(row => new SelectListItem {Text = row.mName, Value = row.Id.ToString(), Selected = mParentId == row.Id}));
        return pParents;
    }

    public static List<SelectListItem> WidgetZones(string pageId, string sName)
    {
        dynamic masterPage;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select pMasterPage From Pages Where pId=@0";
            masterPage = db.QueryValue(sqlSelect, pageId);
        }
        var context = HttpContext.Current;
        var textFile = new StreamReader(context.Server.MapPath("~/Layouts/" + masterPage));
        var fileContents = textFile.ReadToEnd();
        textFile.Close();
        var availZones = new System.Collections.ArrayList();
        const string pattern = "@RenderSection\\(\"[\\w]*";
        foreach (Match m in Regex.Matches(fileContents, pattern))
        {
            availZones.Add(m.ToString().Replace("@RenderSection(\"", ""));
        }
        var arr = availZones.ToArray();
        var distZones = arr.Distinct();
        return distZones.Select(zone => new SelectListItem {Text = CleanNames(zone.ToString()), Value = zone.ToString(), Selected = sName == zone.ToString()}).ToList();
    }

    public static List<SelectListItem> AvailableWidgets(string wId)
    {
        IEnumerable<dynamic> widgets;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelectWidgets = "Select Id, wName From Widgets Order By wName";
            widgets = db.Query(sqlSelectWidgets);
        }
        return widgets.Select(row => new SelectListItem {Text = row.wName, Value = row.Id.ToString(), Selected = wId.AsInt() == row.Id}).ToList();
    }

    public static List<SelectListItem> PageBackups(string pId)
    {
        var pBackups = new List<SelectListItem>();
        IEnumerable<dynamic> backups;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select Id, pwDate From Backups Where bType='p' And pwID=@0 Order By id Desc";
            backups = db.Query(sqlSelect, pId);
        }
        pBackups.Add(new SelectListItem { Text = "Load From Backup", Value = "-1" });
        pBackups.AddRange(backups.Take(5).Select(f => new SelectListItem {Text = f.pwDate.ToString(), Value = f.Id.ToString()}));
        pBackups.Add(new SelectListItem { Text = "Load Current Version", Value = "0" });
        return pBackups;
    }

    public static List<SelectListItem> WidgetBackups(string id)
    {
        var wBackups = new List<SelectListItem>();
        IEnumerable<dynamic> backups;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select Id, pwDate From Backups where bType='w' And pwID=@0 Order By id Desc";
            backups = db.Query(sqlSelect, id);
        }
        wBackups.Add(new SelectListItem { Text = "Load From Backup", Value = "-1" });
        wBackups.AddRange(backups.Take(5).Select(f => new SelectListItem {Text = f.pwDate.ToString(), Value = f.Id.ToString()}));
        wBackups.Add(new SelectListItem { Text = "Load Current Version", Value = "0" });
        return wBackups;
    }

    public static List<string> MenuItems()
    {
        IEnumerable<dynamic> pages;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select pName From Pages Where pHTML=1 Order By pName";
            pages = db.Query(sqlSelect);
        }
        return pages.Select(row => row.pName).Cast<string>().ToList();
    }

    public static string AddonsMenu(string name)
    {
        var retV = string.Concat(name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        return retV;
    }

    public static void EmptyDirectory(this DirectoryInfo directory)
    {
        foreach (var file in directory.GetFiles())
        {
            file.Delete();
        }
        foreach (var subDirectory in directory.GetDirectories())
        {
            subDirectory.Delete(true);
        }
    }

    public static string GetUserId(string username)
    {
        string userId;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select Id From Users Where Username = @0";
            userId = db.QueryValue(sqlSelect, username).ToString();
        }
        return userId;
    }

    public static bool UserExists(string username)
    {
        var exists = false;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select Count(*) From Users Where Username = @0";
            var count = (int)db.QueryValue(sqlSelect, username);
            if (count > 0)
            {
                exists = true;
            }
        }
        return exists;
    }

    public static string GetDisplayName(string username)
    {
        string displayName;
        using (var db = Database.Open(GetDbName()))
        {
            const string sqlSelect = "Select DisplayName From Users Where Username = @0";
            displayName = db.QueryValue(sqlSelect, username);
        }
        return displayName;
    }
}