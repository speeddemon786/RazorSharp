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
        StringBuilder sb = new StringBuilder();

        var Context = new HttpContextWrapper(HttpContext.Current);
        IEnumerable<dynamic> Data = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select * From Menu Order By mOrderId DESC";
            Data = Db.Query(SqlSelect);
        }
        var pageName = Context.GetRouteValue("PageName");
        if (pageName == null)
        {
            pageName = "default";
        }

        pageName = "~/" + pageName.ToLower();
        if (pageName == "~/default")
        {
            pageName = "~/";
        }

        sb.Append("<ul class=\"" + className + "\" id=\"" + id + "\">");
        sb.Append(BuildMenu(Data, 0, pageName));
        sb.Append("</ul>");

        return sb.ToString();
    }

    public static string BuildMenu(IEnumerable<dynamic> menu, int parentid, string pageName)
    {
        var items = menu.Where(m => m.mParentId == parentid);
        StringBuilder sb = new StringBuilder();

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
                    foreach (var child in hasChilds)
                    {
                        if (pageName == child.mURL)
                        {
                            pageName = item.mURL;
                            childPage = child.mURL;
                        }
                    }
                }

                string URL = item.mURL;
                if (item.mURL.ToString().StartsWith("~"))
                {
                    URL = VirtualPathUtility.ToAbsolute(item.mURL);
                }
                sb.Append("<li");
                if (pageName.ToLower() == item.mURL.ToLower())
                {
                    sb.Append(" class=\"active\" ");
                }
                sb.Append(">");
                sb.Append("<a href=\"" + URL + "\" target=\"" + item.mTarget + "\"");
                if (hasChilds.Any())
                {
                    sb.Append(" data-toggle=\"dropdown\" class=\"dropdown-toggle\" >");
                }
                else
                {
                    sb.Append(">");
                }
                sb.Append(item.mName);
                if (hasChilds.Any())
                {
                    sb.Append("<span class=\"caret\"></span></a>");
                }
                else
                {
                    sb.Append("</a>");
                }
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
        else
        {
            return "<a href = \"EditWidget?id=" + id + "\" title=\"Edit Widget\"><i class=\"glyphicon glyphicon-edit\"></i></a>";
        }
    }

    public static string DeleteWidgetImg(int id, string wFile)
    {
        if (!wFile.StartsWith("_addon_"))
        {
            return "<a href = \"AllWidgets?id=" + id + "\" onclick = \"return confirm('Are You Sure You Want To Delete This Widget?')\" title = \"Delete\" ><i class=\"glyphicon glyphicon-remove\"></i></a>";
        }
        else
        {
            return "";
        }
    }

    public static string DeleteWidgetInPage(int pageId, string pageName, int id, string wFile)
    {
        return "<a href=\"WidgetsOnPage?pId=" + pageId + "&pName=" + pageName + "&deleteId=" + id + "\" onclick=\"return confirm('Are You Sure You Want To Delete This Widget?')\" title=\"Delete\"><i class=\"glyphicon glyphicon-remove\" alt=\"Delete\"></i></a>";
    }

    public static string EditMasterPageLink(string pMasterPage)
    {
        return "<a href='" + VirtualPathUtility.ToAbsolute("~/Admin/Editor?fileToEdit=" + HttpContext.Current.Server.MapPath("~/Layouts/") + pMasterPage + "&pMasterPage=" + pMasterPage) + "'>" + pMasterPage + "</a>";
    }

    public static string EditStyleSheetLinks(FileInfo[] fileInfoCSS, string pMasterPage)
    {
        StringBuilder sb = new StringBuilder();
        if (fileInfoCSS != null)
        {
            foreach (var f in fileInfoCSS)
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

    public static string RecentBackups(int Count)
    {
        StringBuilder sb = new StringBuilder();
        IEnumerable<dynamic> Data = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Top " + Count + " pwTitle, pwDate From Backups Order By pwDate DESC";
            Data = Db.Query(SqlSelect);
        }
        sb.Append("<ul class=\"dropdown-menu alert-dropdown\">");
        if (Data.Count() == 0)
        {
            sb.Append("<li>No Backups Found</li>");
        }
        else
        {
            sb.Append("<li class=\"text-center\"><a href=\"#\">Recent Backups</a></li>");
            foreach (var row in Data)
            {
                sb.Append("<li><a href=\"#\">" + Functions.GetShortString(row.pwTitle, 10) + " <span class=\"label label-primary pull-right\">" + string.Format("{0 : dd/MM/yyyy}", row.pwDate) + "</span></a></li>");
            }
        }
        sb.Append("<li class=\"divider\"></li>");
        var BackupCount = 0;
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Count(*) From Backups";
            BackupCount = (int)Db.QueryValue(SqlSelect);
        }
        sb.Append("<li class=\"text-center\"><a href=\"#\">Total Backups : " + BackupCount.ToString() + "</a>");
        sb.Append("</ul>");
        return sb.ToString();
    }

    public static string FooterCopyright()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<p><span class=\"small\">Copyright &copy; " + DateTime.Now.Year + " " + HttpContext.Current.Application["SiteName"] + ".</span>");
        sb.Append("<span class=\"pull-right small\">Powered by Razor Sharp " + HttpContext.Current.Application["Version"] + ".</span></p>");
        return sb.ToString();
    }

    public static string alertSuccess(string Message)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-success\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-ok-sign\"></i>  " + Message);
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string alertInfo(string Message)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-info\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-info-sign\"></i>  " + Message);
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string alertWarning(string Message)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-warning\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-exclamation-sign\"></i>  " + Message);
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string alertDanger(string Message)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<div class=\"alert alert-danger\">");
        sb.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>");
        sb.Append("<i class=\"glyphicon glyphicon-remove-circle\"></i>  " + Message);
        sb.Append("</div>");
        return sb.ToString();
    }
}