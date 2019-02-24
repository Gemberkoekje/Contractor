namespace ContractorCore
{
    public class Location : MongoDbTable<Location>
    {
        public Location ParentLocation;

        public string Name;
        public LocationType LocationType;

    }
}
