namespace PizzaWelt.Services;

public interface ILiveTrackingService : IApplicationService
{
    public Task<LiveTrackingModel> GetLiveTrackingByOrderIdAsync(int id);
}
