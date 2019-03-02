using MongoDB.Bson;
using System.Collections.Generic;

namespace ContractorCore
{
    public class Activity
    {
        public ObjectId _id;
        public string Name;
        public ObjectId Result;
        public List<ObjectId> Input;
        public decimal Effectiveness;
        public int Labor;
    }
}
