using System.Collections.Generic;

namespace ContractorCore
{
    public class Activity : MongoDbTable<Activity>
    {
        public string Name;
        public MongoDbRef<CommodityWithAmount> Result;
        public List<MongoDbRef<CommodityWithAmount>> Input;
        public decimal Effectiveness;
        public int Labor;
    }
}
