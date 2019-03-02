namespace ContractorCore
{
    public class Government : MongoDbTable<Government>
    {
        public MongoDbRef<Location> Location;
        public int Population;
        public decimal Money;
    }
}
