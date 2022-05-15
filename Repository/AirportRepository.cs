using NearAirports.Models;
using NearAirports.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace NearAirports.Repository
{
    public class AirportRepository
    {
        private readonly IMongoCollection<Aeroporto> airportsCollection;

        public AirportRepository(IOptions<AirportsDatabaseSettings> airportsDatabaseSettings)
        {
            var mongoClient = new MongoClient(airportsDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(airportsDatabaseSettings.Value.DatabaseName);
            airportsCollection = mongoDatabase.GetCollection<Aeroporto>(airportsDatabaseSettings.Value.AirportsCollectionName);
        }

        public async Task<Aeroporto?> GetAsync(FilterDefinition<Aeroporto> filter) => await airportsCollection.Find(filter).FirstOrDefaultAsync();

        public async Task<List<Aeroporto>> GetManyAsync(FilterDefinition<Aeroporto> filter) => await airportsCollection.Find(filter).ToListAsync();
    }
}
