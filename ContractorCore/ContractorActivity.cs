using MongoDB.Bson;

namespace ContractorCore
{
    public class ContractorActivity
    {
        public ObjectId _id;
        public ObjectId Contractor;
        public ObjectId Activity;

        public decimal LaborWages;
        public decimal ResultPrice;
    }
}
