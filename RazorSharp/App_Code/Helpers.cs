using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WebMatrix.Data;

/// <summary>
/// Database generated Helpers for menus and admin grids
/// </summary>

public class Helpers
{
    public static string Menu(string className = null, string id = null)
    {
        var sb = new StringBuilder();

        var context = new HttpContextWrapper(HttpContext.Current);
        IEnumerable<dynamic> data;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select * From Menu Order By mOrderId DESC";
            data = db.Query(sqlSelect);
        }
        var pageName = context.GetRouteValue("PageName") ?? "default";

        pageName = "~/" + pageName.ToLower();
        if (pageName == "~/default")
        {
            pageName = "~/";
        }

        sb.Append("<ul class=\"" + className + "\" id=\"" + id + "\">");
        sb.Append(BuildMenu(data, 0, pageName));
        sb.Append("</ul>");

        return sb.ToString();
    }

    public static string BuildMenu(IEnumerable<dynamic> menu, int parentid, string pageName)
    {
        var items = menu.Where(m => m.mParentId == parentid);
        var sb = new StringBuilder();

        if (items.Any())
        {
            if (items.First().mParentId > 0)
            {
                sb.Append("<ul class=\"dropdown-menu\" role=\"menu\" >");
            }

            foreach (var item in items)
            {
                var hasChilds = menu.Where(m => m.mParentId == item.Id);
                var childPage = "";
                if (hasChilds.Any())
                {
                    foreach (var child in hasChilds.Where(child => pageName == child.mURL))
                    {
                        pageName = item.mURL;
                        childPage = child.mURL;
                    }
                }

                string url = item.mURL;
                if (item.mURL.ToString().StartsWith("~"))
                {
                    url = VirtualPathUtility.ToAbsolute(item.mURL);
                }
                sb.Append("<li");
                if (pageName.ToLower() == item.mURL.ToLower())
                {
                    sb.Append(" class=\"active\" ");
                }
                sb.Append(">");
                sb.Append("<a href=\"" + url + "\" target=\"" + item.mTarget + "\"");
                sb.Append(hasChilds.Any() ? " data-toggle=\"dropdown\" class=\"dropdown-toggle\" >" : ">");
                sb.Append(item.mName);
                sb.Append(hasChilds.Any() ? "<span class=\"caret\"></span></a>" : "</a>");
                sb.Append(BuildMenu(menu, item.Id, childPage));
                sb.Append("</li>");
            }

            if (items.First().mParentId > 0)
            {
                sb.Append("</ul>");
            }
        }
        return sb.ToString();
    }

    public static string EditWidgetImg(int id, string wFile)
    {
        if (wFile.StartsWith("_addon_"))
        {
            return "<i class=\"glyphicon glyphicon-list-alt\" title=\"Go To Addons To Edit\"></i>";
        }
        return "<a href = \"EditWidget?id=" + id + "\" title=\"Edit Widget\"><i class=\"glyphicon glyphicon-edit\"></i></a>";
    }

    public static string DeleteWidgetImg(int id, string wFile)
    {
        if (!wFile.StartsWith("_addon_"))
        {
            return "<a href = \"AllWidgets?id=" + id + "\" onclick = \"return confirm('Are You Sure You Want To Delete This Widget?')\" title = \"Delete\" ><i class=\"glyphicon glyphicon-remove\"></i></a>";
        }
        return "";
    }

    public static string DeleteWidgetInPage(int pageId, string pageName, int id, string wFile)
    {
        return "<a href=\"WidgetsOnPage?pId=" + pageId + "&pName=" + pageName + "&deleteId=" + id + "\" onclick=\"return confirm('Are You Sure You Want To Delete This Widget?')\" title=\"Delete\"><i class=\"glyphicon glyphicon-remove\" alt=\"Delete\"></i></a>";
    }

    public static string EditMasterPageLink(string pMasterPage)
    {
        return "<a href='" + VirtualPathUtility.ToAbsolute("~/Admin/Editor?fileToEdit=" + HttpContext.Current.Server.MapPath("~/Layouts/") + pMasterPage + "&pMasterPage=" + pMasterPage) + "'>" + pMasterPage + "</a>";
    }

    public static string EditStyleSheetLinks(FileInfo[] fileInfoCss, string pMasterPage)
    {
        var sb = new StringBuilder();
        if (fileInfoCss != null)
        {
            foreach (var f in fileInfoCss)
            {
                sb.Append("<a href = '" + VirtualPathUtility.ToAbsolute("~/Admin/Editor?fileToEdit=" + f.DirectoryName + "/" + f.Name + "&pMasterPage=" + pMasterPage) + "' >" + f.Name + "</a> , ");
            }
        }
        else
        {
            sb.Append("No CSS Files Detected");
        }
        return sb.ToString();
    }

    public static string RecentBackups(int count)
    {
        var sb = new StringBuilder();
        IEnumerable<dynamic> data;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            var sqlSelect = "Select Top " + count + " pwTitle, pwDate From Backups Order By pwDate DESC";
            data = db.Query(sqlSelect);
        }
        sb.Append("<ul class=\"dropdown-menu alert-dropdown\">");
        if (!data.Any())
        {
            sb.Append("<li>No Backups Found</li>");
        }
        else
        {
            sb.Append("<li class=\"text-center\"><a href=\"#\">Recent Backups</a></li>");
            foreach (var row in data)
            {
                sb.Append("<li><a href=\"#\">" + Functions.GetShortString(row.pwTitle, 10) + " <span class=\"label label-primary pull-right\">" + string.Format("{0 : dd/MM/yyyy}", row.pwDate) + "</span></a></li>");
            }
        }
        sb.Append("<li class=\"divider\"></li>");
        int backupCount;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select Count(*) From Backups";
            backupCount = (int)db.QueryValue(sqlSelect);
        }
        sb.Append("<li class=\"text-center\"><a href=\"#\">Total Backups : " + backupCount + "</a>");
        sb.Append("</ul>");
        return sb.ToString();
    }

    public static string FooterCopyright()
    {
        var sb = new StringBuilder();
        sb.Append("<p><span class=\"small\">Copyright &copy; " + DateTime.Now.Year + " " + HttpContext.Current.Application["SiteName"] + ".</span>");
        sb.Append("<span class=\"pull-right small\">Powered by Razor Sharp " + HttpContext.Current.Application["Version"] + ".</span></p>");
        return sb.ToString();
    }

    public static string AlertSuccess(string message)
    {
        var sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-success\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-ok-sign\"></i>  " + message);
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string AlertInfo(string message)
    {
        var sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-info\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-info-sign\"></i>  " + message);
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string AlertWarning(string message)
    {
        var sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-warning\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-exclamation-sign\"></i>  " + message);
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string AlertDanger(string message)
    {
        var sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-danger\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-remove-circle\"></i>  " + message);
        sb.Append("</div>");
        return sb.ToString();
    }
}