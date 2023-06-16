namespace pizzaWelt.WebApi.Models;

public class LiveTrackingModel
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? CurrentLatOfDriver { get; set; }
    public string? CurrentLonOfDriver { get; set; }
    public string? DestiantionLat { get; set; }
    public string? DestiantionLon { get; set; }
    public string? EstimatedTimeUntilDelivery { get; set; }
    public string? KmLeft { get; set; }
    public bool DriverIsOnTheWay { get; set; }
    public string? Status { get; set; }
}
