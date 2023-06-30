using System.Text.Json;

namespace pizzaWelt.Models;



public class Rootobject
{
    public Location[]? LocationSet { get; set; }
}

public class Location
{
    public string? CurrentLocation { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
}


