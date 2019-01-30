using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Diagnostics;

namespace ContractorCore
{
    public class Class1
    {
        public Class1()
        {
            var map1 = BsonClassMap.RegisterClassMap<Location>();
            var map2 = BsonClassMap.RegisterClassMap<Contractor>();

            var collection = DataProvider.GetDatabase().GetCollection<Contractor>("Contractors");
            var location = new Location()
            {
                Name = "Europe",
                LocationType = LocationType.Continent
            };
            var test = new MongoDbRef<Location>();
            test.Set(location);
            var contractor = new Contractor() { Name = "Test", Location = test };
            try
            {
                collection.InsertOne(contractor);
                foreach (var document in collection.Find(new BsonDocument()).ToList())
                {
                    Console.WriteLine(document.ToBsonDocument().ToString());
                    Console.WriteLine(document.Location.Get().ToBsonDocument().ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
