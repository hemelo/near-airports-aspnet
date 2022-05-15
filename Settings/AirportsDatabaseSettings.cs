namespace NearAirports.Settings
{
    public class AirportsDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string AirportsCollectionName { get; set; } = null!;
    }

    public class MapsApiSettings
    {
        public string ApiKey { get; set; } = null!;
    }
}

