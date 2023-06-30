namespace pizzaWelt.WebApi.Models;

public class LiveTrackingModel
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? CurrentLatOfDriver { get; set; }
    public string? CurrentLonOfDriver { get; set; }
    public float? DestiantionLat { get; set; }
    public float? DestiantionLon { get; set; }
    public double? EstimatedTimeUntilDelivery { get; set; }
    public double? KmLeft { get; set; }
    public bool DriverIsOnTheWay { get; set; }
    public string? Status { get; set; }
}


