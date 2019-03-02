using System;
using System.Collections.Generic;
using System.Text;

namespace ContractorCore
{
    public class CommodityWithAmount : MongoDbTable<CommodityWithAmount>
    {
        public MongoDbRef<Commodity> Commodity;
        public int Amount;
    }
}
