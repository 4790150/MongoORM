using MongoDB.Bson;
using Test;

public static class MongoUtility2
{
    public static bool Parse(BsonValue bsonValue, out BsonList<int> list)
    {
        BsonDocument document = bsonValue.AsBsonDocument;

        list = new BsonList<int>();
        foreach (var pair in document)
        {
            BsonDocument itemDocument = pair.Value.AsBsonDocument;
            int key = int.Parse(pair.Name);
            int prevKey = (int)itemDocument["PrevKey"];
            int value = (int)itemDocument["Value"];
            BsonListElement<int> item = new BsonListElement<int>(key, prevKey, value);
            list.ItemList.Add(item);
        }
        return true;
    }

    public static BsonValue ToBsonValue(this BsonList<int> list)
    {
        int prevKey = 0;
        BsonDocument document = new BsonDocument();
        foreach (var item in list.ItemList)
        {
            BsonDocument subDocument = new BsonDocument();
            subDocument["PrevKey"] = prevKey;
            subDocument["Value"] = item.Element;
            document[item.Key.ToString()] = subDocument;

            prevKey = item.Key;
        }
        return document;
    }

    public static bool Parse(BsonValue bsonValue, out BsonDictionary<int, string> dict)
    {
        BsonDocument document = bsonValue.AsBsonDocument;

        dict = new BsonDictionary<int, string>();
        foreach (var pair in document)
        {
            BsonDocument itemDocument = pair.Value.AsBsonDocument;
            int key = int.Parse(pair.Name);
            string value = (string)itemDocument["Value"];
            dict.Add(key, value);
        }
        dict.ClearState();
        return true;
    }

    public static BsonValue ToBsonValue(this BsonDictionary<int, string> dict)
    {
        BsonDocument document = new BsonDocument();
        dict.Foreach(UpdateState.None | UpdateState.Set, (key, value, state) =>
        {
            document[key.ToString()] = value;
        });
        return document;
    }
}