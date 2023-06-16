using Microsoft.AspNetCore.Mvc;
using pizzaWelt.WebApi.Interfaces;
using pizzaWelt.WebApi.Models;


namespace pizzaWelt.WebApi.Controller;


[Route("api/[controller]")]
[ApiController]
public class LiveTrackingController : ControllerBase
{
    private readonly ILiveTrackingService liveTrackingService;
    public LiveTrackingController(ILiveTrackingService liveTrackingService)
    {
        this.liveTrackingService = liveTrackingService;
    }

    [HttpGet("/api/LiveTracking/OrderId={id}")]
    public async Task<ActionResult<LiveTrackingModel>> GetLiveTrackingByOrderIdAsync(int id)
    {
        var result = await liveTrackingService.GetLiveTrackingByOrderIdAsync(id);
        if (result == null)
            return NotFound("Sorry not found");

        return Ok(result);
    }
    
}


