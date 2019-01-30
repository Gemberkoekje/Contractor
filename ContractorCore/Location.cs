namespace ContractorCore
{
    public class Location : MongoDbTable
    {
        public Location ParentLocation;

        public string Name;
        public LocationType LocationType;

    }
}
