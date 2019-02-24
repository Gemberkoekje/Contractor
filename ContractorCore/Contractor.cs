using MongoDB.Bson;

namespace ContractorCore
{
    public class Contractor : MongoDbTable<Contractor>
    {
        public string Name;
        public MongoDbRef<Location> Location;
    }
}
