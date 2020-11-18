using MongoDB.Bson;
using System;

namespace Test
{
    public partial class Role
    {
        public long RoleID { get; set; }

        private bool _RoleNameDirty;
        private string _RoleName;
        public string RoleName
        {
            get => _RoleName;
            set
            {
                if (value == _RoleName)
                    return;

                _RoleName = value;
                _RoleNameDirty = true;
            }
        }
        public readonly BsonDictionary<int, int> Nums = new BsonDictionary<int, int>();
        public readonly BsonDictionary<int, string> Texts = new BsonDictionary<int, string>();
        public readonly BsonDictionary<int, Item> Items = new BsonDictionary<int, Item>();
        public readonly BsonDictionary<string, string> DictStr = new BsonDictionary<string, string>();
        public readonly BsonDictionary<long, Friend> Friends = new BsonDictionary<long, Friend>();
        public readonly BsonList<int> ListInt = new BsonList<int>();
        public readonly BsonList<string> ListStr = new BsonList<string>();
        public readonly BsonList<Friend> ListFriend = new BsonList<Friend>();

        public bool DataDirty
        {
            get
            {
                if (_RoleNameDirty) return true;
                if (Nums.internalAdded.Count > 0 || Nums.internalRemoved.Count > 0) return true;
                if (Texts.internalAdded.Count > 0 || Texts.internalRemoved.Count > 0) return true;
                if (Items.internalAdded.Count > 0 || Items.internalRemoved.Count > 0) return true;
                foreach (var pair in Items)
                {
                    if (null != pair.Value && pair.Value.DataDirty)
                        return true;
                }
                if (DictStr.internalAdded.Count > 0 || DictStr.internalRemoved.Count > 0) return true;
                if (Friends.internalAdded.Count > 0 || Friends.internalRemoved.Count > 0) return true;
                foreach (var pair in Friends)
                {
                    if (null != pair.Value && pair.Value.DataDirty)
                        return true;
                }
                foreach (var item in ListInt.internalItems)
                {
                    if (item.DataDirty)
                        return true;
                }
                foreach (var item in ListStr.internalItems)
                {
                    if (item.DataDirty)
                        return true;
                }
                foreach (var item in ListFriend.internalItems)
                {
                    if (item.DataDirty)
                        return true;
                    if (null !=  item.Value && item.Value.DataDirty)
                        return true;
                }
                return false;
            }
        }

        public void ClearDirty()
        {
            _RoleNameDirty = false;
            Nums.ClearDirty();
            Texts.ClearDirty();
            Items.ClearDirty();
            foreach (var pair in Items)
                pair.Value?.ClearDirty();
            DictStr.ClearDirty();
            Friends.ClearDirty();
            foreach (var pair in Friends)
                pair.Value?.ClearDirty();
            ListInt.ClearDirty();
            ListStr.ClearDirty();
            ListFriend.ClearDirty();
            foreach (var item in ListFriend)
                item?.ClearDirty();
        }

        public static explicit operator BsonValue(Role item)
        {
            if (null == item)
                return BsonNull.Value;

            return (BsonDocument)item;
        }

        public static explicit operator BsonDocument(Role item)
        {
            if (null == item)
                return null;

            BsonDocument document = new BsonDocument();
            document.Add("_id", item.RoleID);
            if (default(string) != item._RoleName)
                document.Add("RoleName", item._RoleName);

            if (item.Nums.Count > 0)
            {
                BsonDocument bsonNums = new BsonDocument();
                foreach (var pair in item.Nums)
                    bsonNums.Add(pair.Key.ToString(), (BsonValue)pair.Value);
                document.Add("Nums", bsonNums);
            }

            if (item.Texts.Count > 0)
            {
                BsonDocument bsonTexts = new BsonDocument();
                foreach (var pair in item.Texts)
                    bsonTexts.Add(pair.Key.ToString(), (BsonValue)pair.Value);
                document.Add("Texts", bsonTexts);
            }

            if (item.Items.Count > 0)
            {
                BsonDocument bsonItems = new BsonDocument();
                foreach (var pair in item.Items)
                    bsonItems.Add(pair.Key.ToString(), (BsonValue)pair.Value);
                document.Add("Items", bsonItems);
            }

            if (item.DictStr.Count > 0)
            {
                BsonDocument bsonDictStr = new BsonDocument();
                foreach (var pair in item.DictStr)
                    bsonDictStr.Add(pair.Key.ToString(), (BsonValue)pair.Value);
                document.Add("DictStr", bsonDictStr);
            }

            if (item.Friends.Count > 0)
            {
                BsonDocument bsonFriends = new BsonDocument();
                foreach (var pair in item.Friends)
                    bsonFriends.Add(pair.Key.ToString(), (BsonValue)pair.Value);
                document.Add("Friends", bsonFriends);
            }

            if (item.ListInt.Count > 0)
            {
                BsonDocument bsonListInt = new BsonDocument();
                for (int i = 0; i < item.ListInt.Count; i++)
                {
                    var value = item.ListInt.internalItems[i];
                    BsonDocument valueDoc = new BsonDocument();
                    valueDoc["PrevKey"] = value.PrevKey;
                    valueDoc["Value"] = (BsonValue)value.Value;
                    bsonListInt[value.Key.ToString()] = valueDoc;
                }
                document.Add("ListInt", bsonListInt);
            }

            if (item.ListStr.Count > 0)
            {
                BsonDocument bsonListStr = new BsonDocument();
                for (int i = 0; i < item.ListStr.Count; i++)
                {
                    var value = item.ListStr.internalItems[i];
                    BsonDocument valueDoc = new BsonDocument();
                    valueDoc["PrevKey"] = value.PrevKey;
                    valueDoc["Value"] = (BsonValue)value.Value;
                    bsonListStr[value.Key.ToString()] = valueDoc;
                }
                document.Add("ListStr", bsonListStr);
            }

            if (item.ListFriend.Count > 0)
            {
                BsonDocument bsonListFriend = new BsonDocument();
                for (int i = 0; i < item.ListFriend.Count; i++)
                {
                    var value = item.ListFriend.internalItems[i];
                    BsonDocument valueDoc = new BsonDocument();
                    valueDoc["PrevKey"] = value.PrevKey;
                    valueDoc["Value"] = (BsonValue)value.Value;
                    bsonListFriend[value.Key.ToString()] = valueDoc;
                }
                document.Add("ListFriend", bsonListFriend);
            }
            return document;
        }

        public static explicit operator Role(BsonValue bsonValue)
        {
            if (bsonValue.IsBsonNull)
                return null;

            return (Role)bsonValue.AsBsonDocument;
        }

        public static explicit operator Role(BsonDocument document)
        {
            Role item = new Role();
            foreach (var field in document)
            {
                switch(field.Name)
                {
                    case "_id": item.RoleID = (long)field.Value; break;
                    case "RoleName": item._RoleName = (string)field.Value; break;

                    case "Nums":
                        foreach (var pair in field.Value.AsBsonDocument)
                            item.Nums.Add(int.Parse(pair.Name), (int)pair.Value);
                        break;

                    case "Texts":
                        foreach (var pair in field.Value.AsBsonDocument)
                            item.Texts.Add(int.Parse(pair.Name), (string)pair.Value);
                        break;

                    case "Items":
                        foreach (var pair in field.Value.AsBsonDocument)
                            item.Items.Add(int.Parse(pair.Name), (Item)pair.Value);
                        break;

                    case "DictStr":
                        foreach (var pair in field.Value.AsBsonDocument)
                            item.DictStr.Add(pair.Name, (string)pair.Value);
                        break;

                    case "Friends":
                        foreach (var pair in field.Value.AsBsonDocument)
                            item.Friends.Add(long.Parse(pair.Name), (Friend)pair.Value);
                        break;

                    case "ListInt":
                        foreach (var pair in field.Value.AsBsonDocument)
                        {
                            BsonDocument valueDoc = pair.Value.AsBsonDocument;
                            int key = int.Parse(pair.Name);
                            int prevKey = (int)valueDoc["PrevKey"];
                            int element = (int)valueDoc["Value"];
                            item.ListInt.internalAdd(new BsonList<int>.Element(key, prevKey, element));
                        }
                        item.ListInt.internalPostDeserialize();
                        break;

                    case "ListStr":
                        foreach (var pair in field.Value.AsBsonDocument)
                        {
                            BsonDocument valueDoc = pair.Value.AsBsonDocument;
                            int key = int.Parse(pair.Name);
                            int prevKey = (int)valueDoc["PrevKey"];
                            string element = (string)valueDoc["Value"];
                            item.ListStr.internalAdd(new BsonList<string>.Element(key, prevKey, element));
                        }
                        item.ListStr.internalPostDeserialize();
                        break;

                    case "ListFriend":
                        foreach (var pair in field.Value.AsBsonDocument)
                        {
                            BsonDocument valueDoc = pair.Value.AsBsonDocument;
                            int key = int.Parse(pair.Name);
                            int prevKey = (int)valueDoc["PrevKey"];
                            Friend element = (Friend)valueDoc["Value"];
                            item.ListFriend.internalAdd(new BsonList<Friend>.Element(key, prevKey, element));
                        }
                        item.ListFriend.internalPostDeserialize();
                        break;
                }
            }
            return item;
        }

        public void Update(UpdateContext context, string path = null)
        {
            if (_RoleNameDirty)
            {
                if (default(string) == _RoleName)
                    context.Unset.Add($"{path}RoleName", 1);       // 如果是默认值则删除该字段,正好解决string==null时的异常
                else
                    context.Set.Add($"{path}RoleName", _RoleName);
            }

            if (0 == Nums.Count && Nums.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}Nums", 1);
            }
            else
            {
                foreach(var key in Nums.internalRemoved)
                    context.Unset.Add($"{path}Nums.{key}", 1);
                foreach (var pair in Nums)
                {
                    if (Nums.internalAdded.Contains(pair.Key))
                        context.Set.Add($"{path}Nums.{pair.Key}", (BsonValue)pair.Value);
                }
            }

            if (0 == Texts.Count && Texts.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}Texts", 1);
            }
            else
            {
                foreach(var key in Texts.internalRemoved)
                    context.Unset.Add($"{path}Texts.{key}", 1);
                foreach (var pair in Texts)
                {
                    if (Texts.internalAdded.Contains(pair.Key))
                        context.Set.Add($"{path}Texts.{pair.Key}", (BsonValue)pair.Value);
                }
            }

            if (0 == Items.Count && Items.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}Items", 1);
            }
            else
            {
                foreach(var key in Items.internalRemoved)
                    context.Unset.Add($"{path}Items.{key}", 1);
                foreach (var pair in Items)
                {
                    if (Items.internalAdded.Contains(pair.Key))
                        context.Set.Add($"{path}Items.{pair.Key}", (BsonValue)pair.Value);
                    else if (null != pair.Value)
                        pair.Value.Update(context, $"{path}Items.{pair.Key}.");
                }
            }

            if (0 == DictStr.Count && DictStr.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}DictStr", 1);
            }
            else
            {
                foreach(var key in DictStr.internalRemoved)
                    context.Unset.Add($"{path}DictStr.{key}", 1);
                foreach (var pair in DictStr)
                {
                    if (DictStr.internalAdded.Contains(pair.Key))
                        context.Set.Add($"{path}DictStr.{pair.Key}", (BsonValue)pair.Value);
                }
            }

            if (0 == Friends.Count && Friends.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}Friends", 1);
            }
            else
            {
                foreach(var key in Friends.internalRemoved)
                    context.Unset.Add($"{path}Friends.{key}", 1);
                foreach (var pair in Friends)
                {
                    if (Friends.internalAdded.Contains(pair.Key))
                        context.Set.Add($"{path}Friends.{pair.Key}", (BsonValue)pair.Value);
                    else if (null != pair.Value)
                        pair.Value.Update(context, $"{path}Friends.{pair.Key}.");
                }
            }

            if (0 == ListInt.Count && ListInt.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}ListInt", 1);
            }
            else
            {
                foreach (var key in ListInt.internalRemoved)
                {
                    context.Unset.Add($"{path}ListInt.{key}", 1);
                }
                foreach (var element in ListInt.internalItems)
                {
                    if (element.NewData)
                    {
                        BsonDocument valueDoc = new BsonDocument();
                        valueDoc["PrevKey"] = element.PrevKey;
                        valueDoc["Value"] = (BsonValue)element.Value;
                        context.Set.Add($"{path}ListInt.{element.Key}", valueDoc);
                    }
                    else
                    {
                        if (element.PrevKeyDirty)
                            context.Set.Add($"{path}ListInt.{element.Key}.PrevKey", element.PrevKey);

                        if (element.ValueDirty)
                            context.Set.Add($"{path}ListInt.{element.Key}.Value", (BsonValue)element.Value);
                    }
                }
            }
            

            if (0 == ListStr.Count && ListStr.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}ListStr", 1);
            }
            else
            {
                foreach (var key in ListStr.internalRemoved)
                {
                    context.Unset.Add($"{path}ListStr.{key}", 1);
                }
                foreach (var element in ListStr.internalItems)
                {
                    if (element.NewData)
                    {
                        BsonDocument valueDoc = new BsonDocument();
                        valueDoc["PrevKey"] = element.PrevKey;
                        valueDoc["Value"] = (BsonValue)element.Value;
                        context.Set.Add($"{path}ListStr.{element.Key}", valueDoc);
                    }
                    else
                    {
                        if (element.PrevKeyDirty)
                            context.Set.Add($"{path}ListStr.{element.Key}.PrevKey", element.PrevKey);

                        if (element.ValueDirty)
                            context.Set.Add($"{path}ListStr.{element.Key}.Value", (BsonValue)element.Value);
                    }
                }
            }
            

            if (0 == ListFriend.Count && ListFriend.internalRemoved.Count > 0)
            {
                context.Unset.Add($"{path}ListFriend", 1);
            }
            else
            {
                foreach (var key in ListFriend.internalRemoved)
                {
                    context.Unset.Add($"{path}ListFriend.{key}", 1);
                }
                foreach (var element in ListFriend.internalItems)
                {
                    if (element.NewData)
                    {
                        BsonDocument valueDoc = new BsonDocument();
                        valueDoc["PrevKey"] = element.PrevKey;
                        valueDoc["Value"] = (BsonValue)element.Value;
                        context.Set.Add($"{path}ListFriend.{element.Key}", valueDoc);
                    }
                    else
                    {
                        if (element.PrevKeyDirty)
                            context.Set.Add($"{path}ListFriend.{element.Key}.PrevKey", element.PrevKey);

                        if (element.ValueDirty)
                            context.Set.Add($"{path}ListFriend.{element.Key}.Value", (BsonValue)element.Value);
                        else if (null != element.Value)
                            element.Value.Update(context, $"{path}ListFriend.{element.Key}.Value.");
                    }
                }
            }
            
        }
    }
}
