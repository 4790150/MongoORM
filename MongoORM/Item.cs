using MongoDB.Bson;
using System;
using Test;

namespace Test
{

    public class Item
    {
        public int ItemUID { get; set; }

        private bool _ItemIDDirty;
        private int _ItemID;
        public int ItemID
        {
            get => _ItemID;
            set
            {
                if (value == _ItemID)
                    return;

                _ItemID = value;
                _ItemIDDirty = true;
            }
        }
        public readonly BsonList<int> Stones = new BsonList<int>();
        public bool DataDirty
        {
            get
            {
                bool dirty = false;
                if (_ItemIDDirty) return true;
                foreach (var item in Stones.ItemList)
                {
                    if (item.Dirty)
                        return true;
                }
                return false;
            }
        }

        public void ClearState()
        {
            _ItemIDDirty = false;
            Stones.ClearState();
        }

        public static explicit operator BsonValue(Item item)
        {
            BsonDocument sb = new BsonDocument();
            sb.Add("ItemUID", item.ItemUID);
            sb.Add("ItemID", item._ItemID);

            BsonArray bsonStones = new BsonArray();
            for (int i = 0; i < item.Stones.Count; i++)
            {
                bsonStones.Add(item.Stones[i]);
            }
            sb.Add("Stones", bsonStones);
            return sb;
        }

        public static explicit operator Item(BsonValue bsonValue)
        {
            BsonDocument document = bsonValue.AsBsonDocument;
            
            Item item = new Item();
            item.ItemUID = (int)document["ItemUID"];
            item._ItemID = (int)document["ItemID"];
            foreach (var pair in document["Stones"].AsBsonDocument)
            {
                BsonDocument valueDoc = pair.Value.AsBsonDocument;
                int key = int.Parse(pair.Name);
                int prevKey = (int)valueDoc["PrevKey"];
                int element = (int)valueDoc["Value"];
                item.Stones.ItemList.Add(new BsonListElement<int>(key, prevKey, element));
            }
            return item;
        }

        public void Save(string path, MongoSql sql)
        {
            if (_ItemIDDirty)
            {
                sql.Set.Add($"{path}ItemID", _ItemID);
            }
            for (int i = 0; i < Stones.Count; i++)
            {
                if (Stones.IsModifyed(i))
                {
                    sql.Set.Add($"{path}Stones.{Stones.ItemList[i].Key}", (BsonValue)Stones[i]);
                }
            }
            
        }
    }
}
