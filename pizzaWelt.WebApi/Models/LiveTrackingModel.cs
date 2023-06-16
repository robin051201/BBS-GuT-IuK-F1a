namespace pizzaWelt.WebApi.Models;

public class LiveTrackingModel
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public double CurrentLatOfDriver { get; set; }
    public double CurrentLonOfDriver { get; set; }
    public decimal EstimatedTimeUntilDelivery { get; set; }
    public double EstimatedClockTime { get; set; }
    public bool DriverIsOnTheWay { get; set; }
    public string? Status { get; set; }
}
