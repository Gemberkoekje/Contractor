﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractorCore
{
    public class CommodityWithAmount
    {
        public ObjectId _id;
        public ObjectId Commodity;
        public int Amount;
    }
}
