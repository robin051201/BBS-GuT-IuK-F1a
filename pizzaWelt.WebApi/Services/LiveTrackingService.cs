using Newtonsoft.Json;
using pizzaWelt.WebApi.Interfaces;
using pizzaWelt.WebApi.Models;

namespace pizzaWelt.WebApi.Services;


internal class LiveTrackingService : ILiveTrackingService
{
    public async Task<LiveTrackingModel?> GetLiveTrackingByOrderIdAsync(int id)
    {
        await GetUserGpsAsync(id);

        LiveTrackingModel trackingModel = new();
        //db get properties by order id
        if(id == 1)
        {
            trackingModel.Id = 1;
            trackingModel.OrderId = 1;
            trackingModel.CurrentLatOfDriver = 49.759191;   //HardCoded
            trackingModel.CurrentLonOfDriver = 6.635281;    //HardCoded
            trackingModel.EstimatedTimeUntilDelivery = 10;
            trackingModel.EstimatedClockTime = 12.15;
            trackingModel.DriverIsOnTheWay = true;
            trackingModel.Status = "In Time";
        }


        return trackingModel;
    }

    public async Task GetUserGpsAsync(int id)
    {
        using var httpClient = new HttpClient();
        int postCode = 54346;
        string street = "Deierbachstraße";
        int houseNumber = 9;
        var apiKey = "35485fbebd224a7baa42d7fa23c0d733";

        var apiCoordinatesUrl = $"https://api.geoapify.com/v1/geocode/search?postcode={postCode}&street={street}&housenumber={houseNumber}&format=json&apiKey={apiKey}";

        try
        {
            var responseCoordinates = await httpClient.GetAsync(apiCoordinatesUrl);

            if (responseCoordinates.IsSuccessStatusCode)
            {
                var contentCoordinates = await responseCoordinates.Content.ReadAsStringAsync();
                //var cities = JsonConvert.DeserializeObject<CityModel[]>(contentCoordinates);

                //if (cities!.Length > 0)
                //{
                //    cityModel.Lat = cities[0].Lat;
                //    cityModel.Lon = cities[0].Lon;
                //}
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
}
