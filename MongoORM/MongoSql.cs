using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class MongoSql
    {
        public BsonDocument Bson = new BsonDocument();
        public BsonDocument Set = new BsonDocument();
        public BsonDocument Unset = new BsonDocument();

        public void Clear()
        {
            Bson.Clear();
            Set.Clear();
            Unset.Clear();
        }

        public BsonDocument ToSql()
        {
            if (Bson.ElementCount > 0)
                return Bson;

            if (Set.ElementCount > 0)
            {
                Bson.Add("$set", Set);
            }
            if (Unset.ElementCount > 0)
            {
                Bson.Add("$unset", Unset);
            }
            return Bson;
        }
    }
}
