using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractorCore
{
    public class Stockpile
    {
        public ObjectId _id;
        public ObjectId Commodity;
        public ObjectId Location;
        public ObjectId Contractor;
        public int Amount;
        public decimal CurrentPrice;
        public decimal ManufactoryPrice;
    }
}
