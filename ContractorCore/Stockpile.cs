using System;
using System.Collections.Generic;
using System.Text;

namespace ContractorCore
{
    public class Stockpile : MongoDbTable<Stockpile>
    {
        public MongoDbRef<Commodity> Commodity;
        public MongoDbRef<Location> Location;
        public int Amount;
        public decimal CurrentPrice;
        public decimal ManufactoryPrice;
    }
}
