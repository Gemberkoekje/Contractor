using MongoDB.Bson;

namespace ContractorCore
{
    public class Location
    {
        public ObjectId _id;
        public ObjectId ParentLocation;

        public string Name;
        public LocationType LocationType;

    }
}
