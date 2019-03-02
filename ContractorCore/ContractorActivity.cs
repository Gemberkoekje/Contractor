namespace ContractorCore
{
    public class ContractorActivity : MongoDbTable<ContractorActivity>
    {
        public MongoDbRef<Contractor> Contractor;
        public MongoDbRef<Activity> Activity;

        public decimal LaborWages;
        public decimal ResultPrice;
    }
}
