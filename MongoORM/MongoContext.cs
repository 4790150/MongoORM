using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoORM
{
    public class UpdateContext
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

        public BsonDocument Build()
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

    public class BulkWriteContext
    {
        List<BsonDocument> Inserts = new List<BsonDocument>();
        Dictionary<BsonDocument, BsonDocument> Updates = new Dictionary<BsonDocument, BsonDocument>();

        public void Clear()
        {
            Inserts.Clear();
            Updates.Clear();
        }

        public void Insert(BsonDocument document)
        {
            if (document.ElementCount > 0)
                Inserts.Add(document);
        }

        public void Update(BsonDocument filter, BsonDocument update)
        {
            if (update.ElementCount > 0)
                Updates[filter] = update;
        }

        public void Execute(IMongoCollection<BsonDocument> collection)
        {
            if (Inserts.Count > 1 && 0 == Updates.Count) // InsertMany
            {
                collection.InsertMany(Inserts);
            }
            else if (Inserts.Count + Updates.Count > 1)  // BulkWrite
            {
                var writeModels = new List<WriteModel<BsonDocument>>(Inserts.Count + Updates.Count);
                foreach (var insertDoc in Inserts)
                    writeModels.Add(new InsertOneModel<BsonDocument>(insertDoc));
                foreach (var pair in Updates)
                    writeModels.Add(new UpdateOneModel<BsonDocument>(pair.Key, pair.Value));

                collection.BulkWrite(writeModels);
            }
            else
            {
                if (1 == Inserts.Count)
                {
                    collection.InsertOne(Inserts[0]);
                }
                else if (1 == Updates.Count)
                {
                    var enumerator = Updates.GetEnumerator();
                    if (enumerator.MoveNext())
                        collection.UpdateOne(enumerator.Current.Key, enumerator.Current.Value);
                }
            }

            Inserts.Clear();
            Updates.Clear();
        }
    }
}
