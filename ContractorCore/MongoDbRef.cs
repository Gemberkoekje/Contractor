using MongoDB.Bson;
using MongoDB.Driver;

namespace ContractorCore
{
    public class MongoDbRef<T> where T : MongoDbTable
    {
        public ObjectId ID;
        public T Get()
        {
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            return collection.Find(f => f._id == ID).FirstOrDefault();
        }
        public void Set(T value)
        {
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            if (value._id == null)
                value._id = ObjectId.GenerateNewId();
            collection.InsertOne(value);
            ID = value._id;
        }
    }
}
