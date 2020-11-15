using MongoDB.Bson;
using Test;

public static class MongoUtility
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
            subDocument["Value"] = (BsonValue)item.Element;
            document[item.Key.ToString()] = subDocument;

            prevKey = item.Key;
        }
        return document;
    }
    public static bool Parse(BsonValue bsonValue, out BsonList<string> list)
    {
        BsonDocument document = bsonValue.AsBsonDocument;

        list = new BsonList<string>();
        foreach (var pair in document)
        {
            BsonDocument itemDocument = pair.Value.AsBsonDocument;
            int key = int.Parse(pair.Name);
            int prevKey = (int)itemDocument["PrevKey"];
            string value = (string)itemDocument["Value"];
            BsonListElement<string> item = new BsonListElement<string>(key, prevKey, value);
            list.ItemList.Add(item);
        }
        return true;
    }

    public static BsonValue ToBsonValue(this BsonList<string> list)
    {
        int prevKey = 0;
        BsonDocument document = new BsonDocument();
        foreach (var item in list.ItemList)
        {
            BsonDocument subDocument = new BsonDocument();
            subDocument["PrevKey"] = prevKey;
            subDocument["Value"] = (BsonValue)item.Element;
            document[item.Key.ToString()] = subDocument;

            prevKey = item.Key;
        }
        return document;
    }
    public static bool Parse(BsonValue bsonValue, out BsonList<Friend> list)
    {
        BsonDocument document = bsonValue.AsBsonDocument;

        list = new BsonList<Friend>();
        foreach (var pair in document)
        {
            BsonDocument itemDocument = pair.Value.AsBsonDocument;
            int key = int.Parse(pair.Name);
            int prevKey = (int)itemDocument["PrevKey"];
            Friend value = (Friend)itemDocument["Value"];
            BsonListElement<Friend> item = new BsonListElement<Friend>(key, prevKey, value);
            list.ItemList.Add(item);
        }
        return true;
    }

    public static BsonValue ToBsonValue(this BsonList<Friend> list)
    {
        int prevKey = 0;
        BsonDocument document = new BsonDocument();
        foreach (var item in list.ItemList)
        {
            BsonDocument subDocument = new BsonDocument();
            subDocument["PrevKey"] = prevKey;
            subDocument["Value"] = (BsonValue)item.Element;
            document[item.Key.ToString()] = subDocument;

            prevKey = item.Key;
        }
        return document;
    }
    public static bool Parse(BsonValue bsonValue, out BsonDictionary<int, int> dict)
    {
        BsonDocument document = bsonValue.AsBsonDocument;

        dict = new BsonDictionary<int, int>();
        foreach (var pair in document)
        {
            BsonDocument itemDocument = pair.Value.AsBsonDocument;
            int key = int.Parse(pair.Name);
            int value = (int)itemDocument["Value"];
            dict.Add(key, value);
        }
        dict.ClearState();
        return true;
    }

    public static BsonValue ToBsonValue(this BsonDictionary<int, int> dict)
    {
        BsonDocument document = new BsonDocument();
        dict.Foreach(UpdateState.None | UpdateState.Set, (key, value, state) =>
        {
            document[key.ToString()] = (BsonValue)value;
        });
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
            document[key.ToString()] = (BsonValue)value;
        });
        return document;
    }        
    public static bool Parse(BsonValue bsonValue, out BsonDictionary<int, Item> dict)
    {
        BsonDocument document = bsonValue.AsBsonDocument;

        dict = new BsonDictionary<int, Item>();
        foreach (var pair in document)
        {
            BsonDocument itemDocument = pair.Value.AsBsonDocument;
            int key = int.Parse(pair.Name);
            Item value = (Item)itemDocument["Value"];
            dict.Add(key, value);
        }
        dict.ClearState();
        return true;
    }

    public static BsonValue ToBsonValue(this BsonDictionary<int, Item> dict)
    {
        BsonDocument document = new BsonDocument();
        dict.Foreach(UpdateState.None | UpdateState.Set, (key, value, state) =>
        {
            document[key.ToString()] = (BsonValue)value;
        });
        return document;
    }        
    public static bool Parse(BsonValue bsonValue, out BsonDictionary<long, Friend> dict)
    {
        BsonDocument document = bsonValue.AsBsonDocument;

        dict = new BsonDictionary<long, Friend>();
        foreach (var pair in document)
        {
            BsonDocument itemDocument = pair.Value.AsBsonDocument;
            long key = int.Parse(pair.Name);
            Friend value = (Friend)itemDocument["Value"];
            dict.Add(key, value);
        }
        dict.ClearState();
        return true;
    }

    public static BsonValue ToBsonValue(this BsonDictionary<long, Friend> dict)
    {
        BsonDocument document = new BsonDocument();
        dict.Foreach(UpdateState.None | UpdateState.Set, (key, value, state) =>
        {
            document[key.ToString()] = (BsonValue)value;
        });
        return document;
    }        
}
