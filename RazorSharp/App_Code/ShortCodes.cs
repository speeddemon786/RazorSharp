using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Class to replace shortcodes that look like [ShortCode/] with partials from Addons section
/// </summary>

public class ShortCodes
{
    public static string RepaceToken(string data)
    {
        string token = @"\[(.*?)\/\]";
        Regex objRegex = new Regex(token);
        MatchCollection objCol = objRegex.Matches(data);
        foreach (Match item in objCol)
        {
            data = data.Replace(item.Groups[0].Value, BuildControl(item.Groups[1].Value.ToString()));
        }
        return data;
    }

    public static string BuildControl(string paramString)
    {
        string[] paramArr = paramString.Split(' ');
        ArrayList arrList = new ArrayList();
        foreach (string a in paramArr)
        {
            arrList.Add(a);
        }
        if (arrList.Count < 2) arrList.Add("");
        string controlPath = "~/Addons/" + arrList[0] + "/Body/" + arrList[0];
        return controlPath;
    }
}