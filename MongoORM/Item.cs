using MongoDB.Bson;
using System;

namespace Test
{
    public partial class Item
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
                if (_ItemIDDirty) return true;
                foreach (var item in Stones.internalItems)
                {
                    if (item.DataDirty)
                        return true;
                }
                return false;
            }
        }

        public void ClearDirty()
        {
            _ItemIDDirty = false;
            Stones.ClearDirty();
        }

        public static explicit operator BsonValue(Item item)
        {
            if (null == item)
                return BsonNull.Value;

            return (BsonDocument)item;
        }

        public static explicit operator BsonDocument(Item item)
        {
            if (null == item)
                return null;

            BsonDocument document = new BsonDocument();
            document.Add("_id", item.ItemUID);
            if (default(int) != item._ItemID)
                document.Add("ItemID", item._ItemID);

            if (item.Stones.Count > 0)
            {
                BsonDocument bsonStones = new BsonDocument();
                for (int i = 0; i < item.Stones.Count; i++)
                {
                    var value = item.Stones.internalItems[i];
                    BsonDocument valueDoc = new BsonDocument();
                    valueDoc["PrevKey"] = value.PrevKey;
                    valueDoc["Value"] = (BsonValue)value.Value;
                    bsonStones[value.Key.ToString()] = valueDoc;
                }
                document.Add("Stones", bsonStones);
            }
            return document;
        }

        public static explicit operator Item(BsonValue bsonValue)
        {
            if (bsonValue.IsBsonNull)
                return null;

            return (Item)bsonValue.AsBsonDocument;
        }

        public static explicit operator Item(BsonDocument document)
        {
            Item item = new Item();
            foreach (var field in document)
            {
                switch(field.Name)
                {
                    case "_id": item.ItemUID = (int)field.Value; break;
                    case "ItemID": item._ItemID = (int)field.Value; break;

                    case "Stones":
                        foreach (var pair in field.Value.AsBsonDocument)
                        {
                            BsonDocument valueDoc = pair.Value.AsBsonDocument;
                            int key = int.Parse(pair.Name);
                            int prevKey = (int)valueDoc["PrevKey"];
                            int element = (int)valueDoc["Value"];
                            item.Stones.internalAdd(new BsonList<int>.Element(key, prevKey, element));
                        }
                        item.Stones.internalPostDeserialize();
                        break;
                }
            }
            return item;
        }

        public void Update(UpdateContext context, string path = null)
        {
            if (_ItemIDDirty)
            {
                if (default(int) == _ItemID)
                    context.Unset.Add($"{path}ItemID", 1);       // 如果是默认值则删除该字段,正好解决string==null时的异常
                else
                    context.Set.Add($"{path}ItemID", _ItemID);
            }

            if (0 == Stones.Count && Stones.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}Stones", 1);
            }
            else
            {
                foreach (var key in Stones.internalRemoved)
                {
                    context.Unset.Add($"{path}Stones.{key}", 1);
                }
                foreach (var element in Stones.internalItems)
                {
                    if (element.NewData)
                    {
                        BsonDocument valueDoc = new BsonDocument();
                        valueDoc["PrevKey"] = element.PrevKey;
                        valueDoc["Value"] = (BsonValue)element.Value;
                        context.Set.Add($"{path}Stones.{element.Key}", valueDoc);
                    }
                    else
                    {
                        if (element.PrevKeyDirty)
                            context.Set.Add($"{path}Stones.{element.Key}.PrevKey", element.PrevKey);

                        if (element.ValueDirty)
                            context.Set.Add($"{path}Stones.{element.Key}.Value", (BsonValue)element.Value);
                    }
                }
            }
            
        }
    }
}
