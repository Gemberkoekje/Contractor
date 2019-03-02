using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ContractorCore
{
    public class NOMongoDbTable<T> where T : NOMongoDbTable<T>
    {
        public ObjectId _id;

        public static T Get()
        {
            var list = List();
            return list.FirstOrDefault();
        }
        public static T Get(Expression<Func<T, bool>> filter)
        {
            var list = List(filter);
            return list.FirstOrDefault();
        }
        public static void Set(T insertval)
        {
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            collection.InsertOne(insertval);
        }
        public static void Update(T insertval)
        {
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            var update = Builders<T>.Update.Set(u => u, insertval);
            collection.UpdateOne(Builders<T>.Filter.Eq(f => f._id, insertval._id), update);
        }
        public static void Delete(Expression<Func<T, bool>> filter)
        {
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            collection.DeleteOne(filter);
        }
        public static IEnumerable<T> List()
        {
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            var returnvalue = collection.Find(new BsonDocument()).ToList();
            return returnvalue;
        }
        public static IEnumerable<T> List(Expression<Func<T, bool>> filter)
        {
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            var returnvalue = collection.Find(filter).ToList();
            return returnvalue;
        }
    }
}
