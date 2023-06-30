namespace pizzaWelt.WebApi.Models;
public class RootobjectDest
{
    public string[] destination_addresses { get; set; }
    public string[] origin_addresses { get; set; }
    public Row[] rows { get; set; }
    public string status { get; set; }
}

public class Row
{
    public Element[] elements { get; set; }
}

public class Element
{
    public Distance distance { get; set; }
    public Duration duration { get; set; }
    public string origin { get; set; }
    public string destination { get; set; }
    public string status { get; set; }
}

public class Distance
{
    public string text { get; set; }
    public int value { get; set; }
}

public class Duration
{
    public string text { get; set; }
    public int value { get; set; }
}

public class DurationAndDistance
{
    public double? Duration { get; set; }
    public double? Distance { get; set; }
}

public class Results
{
    public int[][] distances { get; set; }
    public int[][] durations { get; set; }
}