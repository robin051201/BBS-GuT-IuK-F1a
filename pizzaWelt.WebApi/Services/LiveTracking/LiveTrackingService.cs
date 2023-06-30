using pizzaWelt.Models;

namespace PizzaWelt.Services;

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
            trackingModel.DestiantionLat = float.Parse(geoCoordinates.lat, CultureInfo.InvariantCulture.NumberFormat);
            trackingModel.DestiantionLon = float.Parse(geoCoordinates.lon, CultureInfo.InvariantCulture.NumberFormat);
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
        GetRandomLocationOfDriver(trackingModel);
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
                var rootObject = JsonConvert.DeserializeObject<Models.Rootobject>(contentCoordinates);

                if (rootObject != null)
                {
                    result.lat = rootObject.results[0].lat;
                    result.lon = rootObject.results[0].lon;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return result;
    }

    public async Task<DurationAndDistance> GetDurationBetweenTwoGps(LiveTrackingModel trackingModel, Result customerLoc)
    {
        DurationAndDistance durationAndDistance = new();

        if (trackingModel != null && customerLoc != null)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://trueway-matrix.p.rapidapi.com/CalculateDrivingMatrix?origins={trackingModel.CurrentLatOfDriver}%2C{trackingModel.CurrentLonOfDriver}&destinations={customerLoc.lat}%2C{customerLoc.lon}"),
                    Headers =
                    {
                        { "X-RapidAPI-Key", "4ea5f971a8mshd68b95e85578838p19b961jsna843abdf6df1" },
                        { "X-RapidAPI-Host", "trueway-matrix.p.rapidapi.com" },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    if (body != null)
                    {
                        Models.Results result = JsonConvert.DeserializeObject<Models.Results>(body);

                        if (result != null)
                        { 
                            int distanceInMeters = result.distances[0][0];
                            double distanceInKilometers = distanceInMeters / 1000.0;
                            int durationInSeconds = result.durations[0][0];
                            double durationInMinutes = durationInSeconds / 60.0;

                            durationAndDistance.Duration = durationInMinutes;
                            durationAndDistance.Distance = distanceInKilometers;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return durationAndDistance;
    }

    public LiveTrackingModel GetRandomLocationOfDriver(LiveTrackingModel trackingModel)
    {
        string jsonFile = "Data/deliveryCoords.json";
        try
        {
            string json = File.ReadAllText(jsonFile);
            Location[] locations = JsonConvert.DeserializeObject<Location[]>(json);

            Random random = new Random();
            if (locations != null)
            {
                int randomIndex = random.Next(0, locations.Length);

                Location randomLocation = locations[randomIndex];

                if (randomLocation != null)
                {
                    trackingModel.CurrentLatOfDriver = randomLocation.Latitude!;
                    trackingModel.CurrentLonOfDriver = randomLocation.Longitude!;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return trackingModel;
    }
}


//ToDo: user wohnort by order id bekommen
