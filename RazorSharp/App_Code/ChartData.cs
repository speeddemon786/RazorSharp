using System.Collections.Generic;

/// <summary>
/// Class with label value pair to populate json for morris charts
/// </summary>
/// 
public class ChartData
{
    public string label { get; set; }
    public int value { get; set; }
}

public class ChartDataList
{
    public List<ChartData> ListOfChartData { get; set; }
}