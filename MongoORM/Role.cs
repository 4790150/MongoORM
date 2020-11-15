using MongoDB.Bson;
using System;
using Test;

namespace Test
{

    public class Role
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
        public readonly BsonDictionary<long, Friend> Friends = new BsonDictionary<long, Friend>();
        public readonly BsonList<int> ListInt = new BsonList<int>();
        public readonly BsonList<string> ListStr = new BsonList<string>();
        public readonly BsonList<Friend> ListFriend = new BsonList<Friend>();
        public bool DataDirty
        {
            get
            {
                bool dirty = false;
                if (_RoleNameDirty) return true;

                dirty = false;
                Nums.Foreach(UpdateState.Set | UpdateState.Unset, (key, item, state) =>
                {
                    dirty = true;
                });
                if (dirty) return true;

                dirty = false;
                Texts.Foreach(UpdateState.Set | UpdateState.Unset, (key, item, state) =>
                {
                    dirty = true;
                });
                if (dirty) return true;

                dirty = false;
                Items.Foreach(UpdateState.None | UpdateState.Set | UpdateState.Unset, (key, item, state) =>
                {
                    if (UpdateState.Set == state || UpdateState.Unset == state)
                    {
                        dirty = true;
                    }
                    else
                    {
                        if (item.DataDirty)
                            dirty = true;
                    }
                });
                if (dirty) return true;

                dirty = false;
                Friends.Foreach(UpdateState.None | UpdateState.Set | UpdateState.Unset, (key, item, state) =>
                {
                    if (UpdateState.Set == state || UpdateState.Unset == state)
                    {
                        dirty = true;
                    }
                    else
                    {
                        if (item.DataDirty)
                            dirty = true;
                    }
                });
                if (dirty) return true;
                foreach (var item in ListInt.ItemList)
                {
                    if (item.Dirty)
                        return true;
                }
                foreach (var item in ListStr.ItemList)
                {
                    if (item.Dirty)
                        return true;
                }
                foreach (var item in ListFriend.ItemList)
                {
                    if (item.Dirty)
                        return true;
                    if (item.Element.DataDirty)
                        return true;
                }
                return false;
            }
        }

        public void ClearState()
        {
            _RoleNameDirty = false;
            Nums.ClearState();
            Texts.ClearState();
            Items.ClearState();
            Friends.ClearState();
            ListInt.ClearState();
            ListStr.ClearState();
            ListFriend.ClearState();
        }

        public static explicit operator BsonValue(Role item)
        {
            BsonDocument sb = new BsonDocument();
            sb.Add("RoleID", item.RoleID);
            sb.Add("RoleName", item._RoleName);

            BsonDocument bsonNums = new BsonDocument();
            item.Nums.Foreach(UpdateState.None | UpdateState.Set, (key, sub, state) =>
            {
                bsonNums.Add(key.ToString(), sub);
            });
            sb.Add("Nums", bsonNums);

            BsonDocument bsonTexts = new BsonDocument();
            item.Texts.Foreach(UpdateState.None | UpdateState.Set, (key, sub, state) =>
            {
                bsonTexts.Add(key.ToString(), sub);
            });
            sb.Add("Texts", bsonTexts);

            BsonDocument bsonItems = new BsonDocument();
            item.Items.Foreach(UpdateState.None | UpdateState.Set, (key, sub, state) =>
            {
                bsonItems.Add(key.ToString(), sub?.ToBsonDocument());
            });
            sb.Add("Items", bsonItems);

            BsonDocument bsonFriends = new BsonDocument();
            item.Friends.Foreach(UpdateState.None | UpdateState.Set, (key, sub, state) =>
            {
                bsonFriends.Add(key.ToString(), sub?.ToBsonDocument());
            });
            sb.Add("Friends", bsonFriends);

            BsonArray bsonListInt = new BsonArray();
            for (int i = 0; i < item.ListInt.Count; i++)
            {
                bsonListInt.Add(item.ListInt[i]);
            }
            sb.Add("ListInt", bsonListInt);

            BsonArray bsonListStr = new BsonArray();
            for (int i = 0; i < item.ListStr.Count; i++)
            {
                bsonListStr.Add(item.ListStr[i]);
            }
            sb.Add("ListStr", bsonListStr);

            BsonArray bsonListFriend = new BsonArray();
            for (int i = 0; i < item.ListFriend.Count; i++)
            {
                bsonListFriend.Add(item.ListFriend[i]?.ToBsonDocument());
            }
            sb.Add("ListFriend", bsonListFriend);
            return sb;
        }

        public static explicit operator Role(BsonValue bsonValue)
        {
            BsonDocument document = bsonValue.AsBsonDocument;
            
            Role item = new Role();
            item.RoleID = (long)document["RoleID"];
            item._RoleName = (string)document["RoleName"];
            foreach (var pair in document["Nums"].AsBsonDocument)
            {
                item.Nums.Add(int.Parse(pair.Name), (int)pair.Value);
            }
            foreach (var pair in document["Texts"].AsBsonDocument)
            {
                item.Texts.Add(int.Parse(pair.Name), (string)pair.Value);
            }
            foreach (var pair in document["Items"].AsBsonDocument)
            {
                item.Items.Add(int.Parse(pair.Name), (Item)pair.Value);
            }
            foreach (var pair in document["Friends"].AsBsonDocument)
            {
                item.Friends.Add(long.Parse(pair.Name), (Friend)pair.Value);
            }
            foreach (var pair in document["ListInt"].AsBsonDocument)
            {
                BsonDocument valueDoc = pair.Value.AsBsonDocument;
                int key = int.Parse(pair.Name);
                int prevKey = (int)valueDoc["PrevKey"];
                int element = (int)valueDoc["Value"];
                item.ListInt.ItemList.Add(new BsonListElement<int>(key, prevKey, element));
            }
            foreach (var pair in document["ListStr"].AsBsonDocument)
            {
                BsonDocument valueDoc = pair.Value.AsBsonDocument;
                int key = int.Parse(pair.Name);
                int prevKey = (int)valueDoc["PrevKey"];
                string element = (string)valueDoc["Value"];
                item.ListStr.ItemList.Add(new BsonListElement<string>(key, prevKey, element));
            }
            foreach (var pair in document["ListFriend"].AsBsonDocument)
            {
                BsonDocument valueDoc = pair.Value.AsBsonDocument;
                int key = int.Parse(pair.Name);
                int prevKey = (int)valueDoc["PrevKey"];
                Friend element = (Friend)valueDoc["Value"];
                item.ListFriend.ItemList.Add(new BsonListElement<Friend>(key, prevKey, element));
            }
            return item;
        }

        public void Save(string path, MongoSql sql)
        {
            if (_RoleNameDirty)
            {
                sql.Set.Add($"{path}RoleName", _RoleName);
            }
            Nums.Foreach(UpdateState.Unset, (key, item, state) =>
            {
                sql.Unset.Add($"{path}Nums.{key}", 1);
            }
            Nums.Foreach(UpdateState.Set, (key, item, state) =>
            {
                sql.Set.Add($"{path}Nums.{key}", (BsonValue)item);
            });
            Texts.Foreach(UpdateState.Unset, (key, item, state) =>
            {
                sql.Unset.Add($"{path}Texts.{key}", 1);
            }
            Texts.Foreach(UpdateState.Set, (key, item, state) =>
            {
                sql.Set.Add($"{path}Texts.{key}", (BsonValue)item);
            });
            Items.Foreach(UpdateState.Unset, (key, item, state) =>
            {
                sql.Unset.Add($"{path}Items.{key}", 1);
            }
            Items.Foreach(UpdateState.None | UpdateState.Set, (key, item, state) =>
            {
                if (UpdateState.Set == state)
                {
                    sql.Set.Add($"{path}Items.{key}", (BsonValue)item);
                }
                else if (item.DataDirty)
                {
                    item.Save($"{path}Items.{key}.", sql);
                }
            });
            Friends.Foreach(UpdateState.Unset, (key, item, state) =>
            {
                sql.Unset.Add($"{path}Friends.{key}", 1);
            }
            Friends.Foreach(UpdateState.None | UpdateState.Set, (key, item, state) =>
            {
                if (UpdateState.Set == state)
                {
                    sql.Set.Add($"{path}Friends.{key}", (BsonValue)item);
                }
                else if (item.DataDirty)
                {
                    item.Save($"{path}Friends.{key}.", sql);
                }
            });
            for (int i = 0; i < ListInt.Count; i++)
            {
                if (ListInt.IsModifyed(i))
                {
                    sql.Set.Add($"{path}ListInt.{ListInt.ItemList[i].Key}", (BsonValue)ListInt[i]);
                }
            }
            
            for (int i = 0; i < ListStr.Count; i++)
            {
                if (ListStr.IsModifyed(i))
                {
                    sql.Set.Add($"{path}ListStr.{ListStr.ItemList[i].Key}", (BsonValue)ListStr[i]);
                }
            }
            
            for (int i = 0; i < ListFriend.Count; i++)
            {
                if (ListFriend.IsModifyed(i))
                {
                    sql.Set.Add($"{path}ListFriend.{ListFriend.ItemList[i].Key}", (BsonValue)ListFriend[i]);
                }
            }
            
        }
    }
}
