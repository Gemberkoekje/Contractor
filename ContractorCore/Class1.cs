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
            var db = DataProvider.GetDatabase();
            var location = new Location()
            {
                Name = "Europe",
                LocationType = LocationType.Continent
            };
            var test = new MongoDbRef<Location>();
            var t = Location.Get(f => f.Name == "Europe");
            if (t != null)
            {
                test.Set(t._id);
            }
            else
            {
                test.SetAndInsert(location);
            }
            var contractor = new Contractor() { Name = "Test", Location = test };
            try
            {
                Contractor.Set(contractor);
                foreach (var document in Contractor.List())
                {
                    Console.WriteLine(document.ToBsonDocument().ToString());
                    Console.WriteLine(document.Location.Get().ToBsonDocument().ToString());
                    Contractor.Delete(f => f._id == document._id);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
