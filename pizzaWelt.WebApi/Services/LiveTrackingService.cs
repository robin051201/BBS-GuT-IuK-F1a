using Newtonsoft.Json;
using pizzaWelt.WebApi.Interfaces;
using pizzaWelt.WebApi.Models;

namespace pizzaWelt.WebApi.Services;


internal class LiveTrackingService : ILiveTrackingService
{
    public async Task<LiveTrackingModel?> GetLiveTrackingByOrderIdAsync(int id)
    {
        LiveTrackingModel trackingModel = new();
        await FillTrackingModel(trackingModel);
        Result geoCoordinates = new();
        DurationAndDistance durationAndDistance = new();
        geoCoordinates = await GetUserGpsAsync(id);
        durationAndDistance = await GetDurationBetweenTwoGps(trackingModel, geoCoordinates);
        if (geoCoordinates != null)
        {
            trackingModel.DestiantionLat = geoCoordinates.lat.ToString();
            trackingModel.DestiantionLon = geoCoordinates.lon.ToString();
        }
        if (durationAndDistance != null)
        {
            trackingModel.EstimatedTimeUntilDelivery = durationAndDistance.Duration;
            trackingModel.KmLeft = durationAndDistance.Distance;
        }

        return trackingModel;
    }
    public async Task<LiveTrackingModel> FillTrackingModel(LiveTrackingModel trackingModel)
    {
        //db get properties by ordeId
        trackingModel.Id = 1;
        trackingModel.OrderId = 1;
        trackingModel.CurrentLatOfDriver = "49.759191";   //HardCoded
        trackingModel.CurrentLonOfDriver = "6.635281";    //HardCoded
        trackingModel.DriverIsOnTheWay = true;
        trackingModel.Status = "In Time";

        return trackingModel;
    }


    public async Task<Result> GetUserGpsAsync(int id)
    {
        using var httpClient = new HttpClient();
        Result result = new();
        var apiKey = "35485fbebd224a7baa42d7fa23c0d733";
        int postCode = 54346;
        string street = "Deierbachstraße";
        int houseNumber = 9;
        var apiCoordinatesUrl = $"https://api.geoapify.com/v1/geocode/search?postcode={postCode}&street={street}&housenumber={houseNumber}&format=json&apiKey={apiKey}";

        try
        {
            var responseCoordinates = await httpClient.GetAsync(apiCoordinatesUrl);

            if (responseCoordinates.IsSuccessStatusCode)
            {
                var contentCoordinates = await responseCoordinates.Content.ReadAsStringAsync();
                var rootObject = JsonConvert.DeserializeObject<Rootobject>(contentCoordinates);

                if (rootObject != null)
                {
                    result.lat = rootObject.results[0].lat;
                    result.lon = rootObject.results[0].lon;
                }

            }
            else
            {
                Console.WriteLine("Error");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return result;
    }

    public async Task<DurationAndDistance> GetDurationBetweenTwoGps(LiveTrackingModel trackingModel, Result geoCoordinates)
    {
        using var httpClient = new HttpClient();
        DurationAndDistance durationAndDistance = new();
        var apiKey = "2ghW21sQpYWDpRavcPQYNQjHqAumo";

        if (trackingModel != null && geoCoordinates != null)
        {
            var apiCoordinatesUrl = $"https://api.distancematrix.ai/maps/api/distancematrix/json?origins={trackingModel.CurrentLatOfDriver},{trackingModel.CurrentLonOfDriver}&destinations={geoCoordinates.lat},{geoCoordinates.lon}&key={apiKey}";

            try
            {
                var responseCoordinates = await httpClient.GetAsync(apiCoordinatesUrl);

                if (responseCoordinates.IsSuccessStatusCode)
                {
                    var contentCoordinates = await responseCoordinates.Content.ReadAsStringAsync();
                    var rootObject = JsonConvert.DeserializeObject<RootobjectDest>(contentCoordinates);

                    if (rootObject != null)
                    {
                        durationAndDistance.Duration = rootObject.rows[0].elements[0].duration.text;
                        durationAndDistance.Distance = rootObject.rows[0].elements[0].distance.text;
                    }

                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return durationAndDistance;
    }

}
