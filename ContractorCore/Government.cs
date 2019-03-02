using MongoDB.Bson;
namespace ContractorCore
{
    public class Government
    {
        public ObjectId _id;
        public ObjectId Location;
        public int Population;
        public decimal Money;
    }
}
