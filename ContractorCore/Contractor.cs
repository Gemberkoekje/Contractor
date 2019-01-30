using MongoDB.Bson;

namespace ContractorCore
{
    public class Contractor
    {
        public ObjectId _id;
        public string Name;
        public MongoDbRef<Location> Location;
    }
}
