using WebMatrix.Data;

/// <summary>
/// Class for managing settings within site
/// </summary>

public class Settings
{
    public static string GetSetting(string settingName)
    {
        string result;
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlSelect = "Select Value From Settings Where Name= @0";
            result = db.QueryValue(sqlSelect, settingName);
        }
        return result;
    }

    public static void UpdateSetting(string settingValue, string settingName)
    {
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlUpdate = "Update Settings Set Value=@0 Where Name=@1";
            db.Execute(sqlUpdate, settingValue, settingName);
        }
    }

    public static void CreateSetting(string settingValue, string settingName)
    {
        using (var db = Database.Open(Functions.GetDbName()))
        {
            const string sqlUpdate = "Insert Into Settings (Name, Value) Values (@0, @1)";
            db.Execute(sqlUpdate, settingName, settingValue);
        }
    }
}