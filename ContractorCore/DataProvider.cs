using MongoDB.Driver;

namespace ContractorCore
{
    public static class DataProvider
    {
        private static IMongoDatabase _database = null;

        public static IMongoDatabase GetDatabase()
        {
            if(_database == null)
            {
                var client = new MongoClient("mongodb://localhost");
                _database = client.GetDatabase("ContractorDb");
            }
            return _database;
        }
    }
}
