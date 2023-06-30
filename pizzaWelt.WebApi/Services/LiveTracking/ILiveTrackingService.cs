using pizzaWelt.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pizzaWelt.WebApi.Interfaces;

public interface ILiveTrackingService
{
    public Task<LiveTrackingModel> GetLiveTrackingByOrderIdAsync(int id);

}
