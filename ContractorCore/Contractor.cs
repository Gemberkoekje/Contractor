using MongoDB.Bson;

namespace ContractorCore
{
    public class Contractor
    {
        public ObjectId _id;
        public string Name;
        public ObjectId Location;
        public decimal Money;
        public decimal ProfitPercentage;
    }
}
