using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Diagnostics;

namespace ContractorCore
{
    public enum LocationType
    {
        Continent,
        Planet,
        SolarSystem,
        MilkyWay
    }

    public class Location
    {
        public ObjectId _id;
        public Location ParentLocation;

        public string Name;
        public LocationType LocationType;

    }

    public class Contractor
    {
        public ObjectId _id;
        public string Name;
        public Location Location;
    }

    public class Class1
    {
        public Class1()
        {
            var client = new MongoClient("mongodb://localhost");
            var database = client.GetDatabase("ContractorDb");
            var map1 = BsonClassMap.RegisterClassMap<Location>();
            var map2 = BsonClassMap.RegisterClassMap<Contractor>();

            var collection = database.GetCollection<Contractor>("Contractors");

            var contractor = new Contractor() { Name = "Test", Location = new Location() { Name = "Europe", LocationType = LocationType.Continent } };
            try
            {
                collection.InsertOne(contractor);
            }catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            var document = collection.Find(new BsonDocument()).FirstOrDefault();
            Console.WriteLine(document.ToString());
        }
    }
}
