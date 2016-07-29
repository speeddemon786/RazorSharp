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
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlDelete = "Delete From Visitors Where TimeStamp < DateAdd(dd,-30,GETDATE())";
            db.Execute(sqlDelete);
        }
    }

    public static string GetCountry(string ipAddress)
    {
        string country;
        using (var client = new WebClient())
        {
            client.Headers.Add("user-agent", "ASP.NET WebClient");
            client.Headers.Add("Content-Type", "application/json");
            try
            {
                var json = client.DownloadString(string.Format("http://www.freegeoip.net/json/{0}", ipAddress));
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
        var context = new HttpContextWrapper(HttpContext.Current);
        var timeStamp = DateTime.Now;
        var page = context.GetRouteValue("PageName") ?? "Default";
        var browser = context.Request.Browser.Browser;
        var browserVersion = context.Request.Browser.Version;
        var operatingsystem = context.Request.Browser.Platform;
        var ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];
        }
        if (ipAddress.Contains(":") && ipAddress != "::1")
        {
            var parts = ipAddress.Split(':');
            ipAddress = parts[0];
        }
        var country = GetCountry(ipAddress);
        string userDns;
        try
        {
            userDns = Dns.GetHostEntry(ipAddress).HostName;
        }
        catch
        {
            userDns = "Unknown";
        }
        var pageReferer = context.Request.ServerVariables["HTTP_REFERER"];
        if (string.IsNullOrEmpty(pageReferer))
        {
            pageReferer = "Direct Link";
        }
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlInsert = "Insert Into Visitors (TimeStamp, Page, Browser, BrowserVersion, OperatingSystem, IPAddress, DNSName, Country, Referer) Values (@0, @1, @2, @3, @4, @5, @6, @7, @8)";
            db.Execute(sqlInsert, timeStamp, page, browser, browserVersion, operatingsystem, ipAddress, userDns, country, pageReferer);
        }
    }

    public ChartDataList GetChartData(string chartName)
    {
        var chartDataList = new ChartDataList {ListOfChartData = new List<ChartData>()};
        var d = new Dictionary<string, int>();
        switch (chartName)
        {
            case "bar":
                var pageViews = GetPageViews();
                foreach (var row in pageViews)
                {
                    d.Add(row.Page, row.Views);
                }
                break;

            case "donut1":
                var browsers = GetBrowserViews();
                foreach (var row in browsers)
                {
                    d.Add(row.Browser, row.Views);
                }
                break;

            case "donut2":
                var operatingSystems = GetOsViews();
                foreach (var row in operatingSystems)
                {
                    d.Add(row.OperatingSystem, row.Views);
                }
                break;

            case "area":
                var dailyViews = GetDailyViews();
                foreach (var row in dailyViews)
                {
                    d.Add(row.VisitDate.ToString("yyyy-MM-dd"), row.Views);
                }
                break;
        }
        foreach (var chartData in d.Select(pair => new ChartData
        {
            label = pair.Key,
            value = pair.Value
        }))
        {
            chartDataList.ListOfChartData.Add(chartData);
        }
        return chartDataList;
    }

    public static IEnumerable<dynamic> GetPageViews()
    {
        IEnumerable<dynamic> pageViews;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select Page, Count(Page) As Views From Visitors Group By Page";
            pageViews = db.Query(sqlSelect);
        }
        return pageViews;
    }

    public static IEnumerable<dynamic> GetBrowserViews()
    {
        IEnumerable<dynamic> browserViews;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select Browser, Count(Browser) As Views From Visitors Group By Browser";
            browserViews = db.Query(sqlSelect);
        }
        return browserViews;
    }

    public static IEnumerable<dynamic> GetOsViews()
    {
        IEnumerable<dynamic> osViews;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select OperatingSystem, Count(OperatingSystem) As Views From Visitors Group By OperatingSystem";
            osViews = db.Query(sqlSelect);
        }
        return osViews;
    }

    public static IEnumerable<dynamic> GetDailyViews()
    {
        IEnumerable<dynamic> dailyViews;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select DateAdd(Day, 0, DateDiff(Day, 0, TimeStamp)) As VisitDate, Count(*) As Views From Visitors Group By DateAdd(Day, 0, DateDiff(Day, 0, TimeStamp))";
            dailyViews = db.Query(sqlSelect);
        }
        return dailyViews;
    }

    public static int RecordCount(string record)
    {
        var sqlCount = string.Empty;
        switch (record)
        {
            case "Browser":
                sqlCount = "Select Count(*) From (Select Distinct Browser From Visitors) x";
                break;

            case "Page":
                sqlCount = "Select Count(*) From (Select Distinct Page From Visitors) x";
                break;

            case "OperatingSystem":
                sqlCount = "Select Count(*) From (Select Distinct OperatingSystem From Visitors) x";
                break;

            case "Total":
                sqlCount = "Select Count(*) From Visitors";
                break;

            case "Referers":
                sqlCount = "Select Count(*) From (Select Distinct Referer From Visitors) x";
                break;

            case "Countries":
                sqlCount = "Select Count(*) From (Select Distinct Country From Visitors) x";
                break;
        }
        using (var db = Database.Open(Functions.GetDbName()))
        {
            var count = (int)db.QueryValue(sqlCount);
            return count;
        }
    }

    public static string ViewsByPage()
    {
        var sb = new StringBuilder();
        IEnumerable<dynamic> pageViews;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select Top 10 Page, Count(Page) As Views From Visitors Group By Page Order By Views Desc";
            pageViews = db.Query(sqlSelect);
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
        var sb = new StringBuilder();
        IEnumerable<dynamic> refererViews;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select Top 10 Referer, Count(Referer) As Views From Visitors Group By Referer Order By Views Desc";
            refererViews = db.Query(sqlSelect);
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
        var sb = new StringBuilder();
        IEnumerable<dynamic> countryViews;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select Top 10 Country, Count(Country) As Views From Visitors Group By Country Order By Views Desc";
            countryViews = db.Query(sqlSelect);
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