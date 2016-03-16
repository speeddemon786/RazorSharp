using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using WebMatrix.Data;

/// <summary>
/// Utility for saving and displaying visitor data
/// </summary>

public class AnalyticsUtility
{
    public static void DeleteOldVisitors()
    {
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlDelete = "Delete From Visitors Where TimeStamp < DateAdd(dd,-30,GETDATE())";
            Db.Execute(SqlDelete);
        }
    }

    static public string GetCountry(string IPAddress)
    {
        var country = string.Empty;
        using (var client = new WebClient())
        {
            client.Headers.Add("user-agent", "ASP.NET WebClient");
            client.Headers.Add("Content-Type", "application/json");
            try
            {
                var json = client.DownloadString(string.Format("http://www.freegeoip.net/json/{0}", IPAddress));
                var serializer = new JavaScriptSerializer();
                var result = serializer.Deserialize<dynamic>(json);
                country = result["country_name"];
            }
            catch (Exception)
            {
                country = string.Empty;
            }
            if (string.IsNullOrEmpty(country))
            {
                country = "Unknown";
            }
        }
        return country;
    }

    public static void SaveVisitorInfo()
    {
        var Context = new HttpContextWrapper(HttpContext.Current);
        DateTime TimeStamp = DateTime.Now;
        var Page = Context.GetRouteValue("PageName");
        if (Page == null)
        {
            Page = "Default";
        }
        string Browser = Context.Request.Browser.Browser;
        string BrowserVersion = Context.Request.Browser.Version;
        string Operatingsystem = Context.Request.Browser.Platform;
        string IPAddress = Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrEmpty(IPAddress))
        {
            IPAddress = Context.Request.ServerVariables["REMOTE_ADDR"];
        }
        if (IPAddress.Contains(":") && IPAddress != "::1")
        {
            var parts = IPAddress.Split(':');
            IPAddress = parts[0];
        }
        string Country = GetCountry(IPAddress);
        string UserDNS = string.Empty;
        try
        {
            UserDNS = Dns.GetHostEntry(IPAddress).HostName;
        }
        catch
        {
            UserDNS = "Unknown";
        }
        string PageReferer = Context.Request.ServerVariables["HTTP_REFERER"];
        if (string.IsNullOrEmpty(PageReferer))
        {
            PageReferer = "Direct Link";
        }
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlInsert = "Insert Into Visitors (TimeStamp, Page, Browser, BrowserVersion, OperatingSystem, IPAddress, DNSName, Country, Referer) Values (@0, @1, @2, @3, @4, @5, @6, @7, @8)";
            Db.Execute(SqlInsert, TimeStamp, Page, Browser, BrowserVersion, Operatingsystem, IPAddress, UserDNS, Country, PageReferer);
        }
    }

    public ChartDataList GetChartData(string chartName)
    {
        ChartData chartData = null;
        ChartDataList chartDataList = new ChartDataList();
        chartDataList.ListOfChartData = new List<ChartData>();
        Dictionary<string, int> d = new Dictionary<string, int>();
        var SqlSelect = string.Empty;
        switch (chartName)
        {
            case "bar":
                IEnumerable<dynamic> PageViews = GetPageViews();
                foreach (var row in PageViews)
                {
                    d.Add(row.Page, row.Views);
                }
                break;

            case "donut1":
                IEnumerable<dynamic> Browsers = GetBrowserViews();
                foreach (var row in Browsers)
                {
                    d.Add(row.Browser, row.Views);
                }
                break;

            case "donut2":
                IEnumerable<dynamic> OperatingSystems = GetOSViews();
                foreach (var row in OperatingSystems)
                {
                    d.Add(row.OperatingSystem, row.Views);
                }
                break;

            case "area":
                IEnumerable<dynamic> DailyViews = GetDailyViews();
                foreach (var row in DailyViews)
                {
                    d.Add(row.VisitDate.ToString("yyyy-MM-dd"), row.Views);
                }
                break;
        }
        foreach (KeyValuePair<string, int> pair in d)
        {
            chartData = new ChartData();
            chartData.label = pair.Key;
            chartData.value = pair.Value;
            chartDataList.ListOfChartData.Add(chartData);
        }
        return chartDataList;
    }

    public static IEnumerable<dynamic> GetPageViews()
    {
        IEnumerable<dynamic> PageViews = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Page, Count(Page) As Views From Visitors Group By Page";
            PageViews = Db.Query(SqlSelect);
        }
        return PageViews;
    }

    public static IEnumerable<dynamic> GetBrowserViews()
    {
        IEnumerable<dynamic> BrowserViews = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Browser, Count(Browser) As Views From Visitors Group By Browser";
            BrowserViews = Db.Query(SqlSelect);
        }
        return BrowserViews;
    }

    public static IEnumerable<dynamic> GetOSViews()
    {
        IEnumerable<dynamic> OSViews = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select OperatingSystem, Count(OperatingSystem) As Views From Visitors Group By OperatingSystem";
            OSViews = Db.Query(SqlSelect);
        }
        return OSViews;
    }

    public static IEnumerable<dynamic> GetDailyViews()
    {
        IEnumerable<dynamic> DailyViews = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select DateAdd(Day, 0, DateDiff(Day, 0, TimeStamp)) As VisitDate, Count(*) As Views From Visitors Group By DateAdd(Day, 0, DateDiff(Day, 0, TimeStamp))";
            DailyViews = Db.Query(SqlSelect);
        }
        return DailyViews;
    }

    public static int RecordCount(string Record)
    {
        var SqlCount = string.Empty;
        switch (Record)
        {
            case "Browser":
                SqlCount = "Select Count(*) From (Select Distinct Browser From Visitors) x";
                break;

            case "Page":
                SqlCount = "Select Count(*) From (Select Distinct Page From Visitors) x";
                break;

            case "OperatingSystem":
                SqlCount = "Select Count(*) From (Select Distinct OperatingSystem From Visitors) x";
                break;

            case "Total":
                SqlCount = "Select Count(*) From Visitors";
                break;

            case "Referers":
                SqlCount = "Select Count(*) From (Select Distinct Referer From Visitors) x";
                break;

            case "Countries":
                SqlCount = "Select Count(*) From (Select Distinct Country From Visitors) x";
                break;
        }
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var Count = (int)Db.QueryValue(SqlCount);
            return Count;
        }
    }

    public static string ViewsByPage()
    {
        StringBuilder sb = new StringBuilder();
        IEnumerable<dynamic> pageViews = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Top 10 Page, Count(Page) As Views From Visitors Group By Page Order By Views Desc";
            pageViews = Db.Query(SqlSelect);
        }
        sb.Append("<div class=\"list-group\">");
        foreach (var row in pageViews)
        {
            sb.Append("<a href=\"#\" class=\"list-group-item\" title=\"" + row.Page + "\">");
            sb.Append("<span class=\"badge\">" + row.Views + "</span> " + Functions.GetShortString(row.Page, 45));
            sb.Append("</a>");
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string ViewsByReferer()
    {
        StringBuilder sb = new StringBuilder();
        IEnumerable<dynamic> refererViews = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Top 10 Referer, Count(Referer) As Views From Visitors Group By Referer Order By Views Desc";
            refererViews = Db.Query(SqlSelect);
        }
        sb.Append("<div class=\"list-group\">");
        foreach (var row in refererViews)
        {
            sb.Append("<a href=\"#\" class=\"list-group-item\" title=\"" + row.Referer + "\">");
            sb.Append("<span class=\"badge\">" + row.Views + "</span> " + Functions.GetShortString(row.Referer, 45));
            sb.Append("</a>");
        }
        sb.Append("</div>");
        return sb.ToString();
    }

    public static string ViewsByCountry()
    {
        StringBuilder sb = new StringBuilder();
        IEnumerable<dynamic> countryViews = Enumerable.Empty<dynamic>();
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Top 10 Country, Count(Country) As Views From Visitors Group By Country Order By Views Desc";
            countryViews = Db.Query(SqlSelect);
        }
        sb.Append("<div class=\"list-group\">");
        foreach (var row in countryViews)
        {
            sb.Append("<a href=\"#\" class=\"list-group-item\" title=\"" + row.Country + "\">");
            sb.Append("<span class=\"badge\">" + row.Views + "</span> " + Functions.GetShortString(row.Country, 45));
            sb.Append("</a>");
        }
        sb.Append("</div>");
        return sb.ToString();
    }
}