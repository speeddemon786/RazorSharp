using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

public static class StaticFile
{
    public static string Version(string rootRelativePath)
    {
        if (HttpRuntime.Cache[rootRelativePath] != null) return HttpRuntime.Cache[rootRelativePath] as string;
        var absolutePath = HostingEnvironment.MapPath(rootRelativePath);
        if (absolutePath == null) return HttpRuntime.Cache[rootRelativePath] as string;
        var lastChangedDateTime = File.GetLastWriteTime(absolutePath);
        if (rootRelativePath.StartsWith("~"))
        {
            rootRelativePath = rootRelativePath.Substring(1);
        }
        var versionedUrl = rootRelativePath + "?v=" + lastChangedDateTime.Ticks;
        HttpRuntime.Cache.Insert(rootRelativePath, versionedUrl, new CacheDependency(absolutePath));
        return HttpRuntime.Cache[rootRelativePath] as string;
    }
}