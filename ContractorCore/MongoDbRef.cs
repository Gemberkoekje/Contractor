﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace ContractorCore
{
    public class MongoDbRef<T> where T : MongoDbTable
    {
        private ObjectId ID;
        public T Get()
        {
            CheckRegistered();
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            return collection.Find(f => f._id == ID).FirstOrDefault();
        }
        public void SetAndInsert(T value)
        {
            CheckRegistered();
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            if (value._id == null)
                value._id = ObjectId.GenerateNewId();
            collection.InsertOne(value);
            ID = value._id;
        }
        public void SetAndUpdate(T value)
        {
            CheckRegistered();
            var collection = DataProvider.GetDatabase().GetCollection<T>(typeof(T).ToString());
            if (value._id == null)
                throw new NotSupportedException("ID needs to be set");
            ID = value._id;
            var update = Builders<T>.Update.Set(u => u, value);
            collection.UpdateOne(Builders<T>.Filter.Eq(f => f._id, ID), update);
        }
        public void Set(ObjectId value)
        {
            ID = value;
        }
        private void CheckRegistered()
        {
            
            if(!BsonClassMap.IsClassMapRegistered(typeof(T)))
                BsonClassMap.RegisterClassMap<T>();
        }
    }
}
