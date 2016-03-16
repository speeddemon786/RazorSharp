using WebMatrix.Data;

/// <summary>
/// Class for managing settings within site
/// </summary>

public class Settings
{
    public static string GetSetting(string SettingName)
    {
        var Result = string.Empty;
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlSelect = "Select Value From Settings Where Name= @0";
            Result = Db.QueryValue(SqlSelect, SettingName);
        }
        return Result;
    }

    public static void UpdateSetting(string SettingValue, string SettingName)
    {
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlUpdate = "Update Settings Set Value=@0 Where Name=@1";
            Db.Execute(SqlUpdate, SettingValue, SettingName);
        }
    }

    public static void CreateSetting(string SettingValue, string SettingName)
    {
        using (var Db = Database.Open(Functions.GetDBName()))
        {
            var SqlUpdate = "Insert Into Settings (Name, Value) Values (@0, @1)";
            Db.Execute(SqlUpdate, SettingName, SettingValue);
        }
    }
}