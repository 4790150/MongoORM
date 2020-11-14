using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public enum UpdateState : byte
    {
        None = 1,
        Set = 1<<1,
        Unset = 1<<2,
        Push = 1<<3,
    }

    class Friend
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

        public bool DataDirty => _RoleNameDirty;


        public BsonDocument ToBsonDocument()
        {
            BsonDocument bson = new BsonDocument();
            bson.Add("RoleID", RoleID);
            bson.Add("RoleName", _RoleName);
            return bson;
        }

        public void Save(string path, MongoSql sql)
        {
            if (_RoleNameDirty)
            {
                sql.Set.Add($"{path}RoleName", _RoleName);
            }
        }
    }

    class Item
    {
        public int ItemUID { get; set; }

        public bool _ItemIDDirty;
        private int _ItemID;
        public int ItemID { get => _ItemID;
            set
            {
                if (value == _ItemID)
                    return;
                _ItemID = value;
                _ItemIDDirty = true;
            }
        }

        public bool DataDirty => _ItemIDDirty;

        public BsonDocument ToBsonDocument()
        {
            BsonDocument sb = new BsonDocument();
            sb.Add("ItemUID", ItemUID);
            sb.Add("ItemID", ItemID);

            BsonArray bsonStones = new BsonArray();
            for (int i = 0; i < stones.Count; i++)
            {
                bsonStones.Add(stones[i]);
            }
            sb.Add("Stones", bsonStones);

            return sb;
        }

        public void Save(string path, MongoSql sql)
        {
            if (_ItemIDDirty)
            {
                sql.Set.Add($"{path}ItemID", _ItemID);
            }

            for (int i = 0; i < stones.Count; i++)
            {
                if (stones.IsModifyed(i))
                {
                    sql.Set.Add($"{path}Stones.{i}", stones[i]);
                }
            }
        }

        private BsonArray<int> stones = new BsonArray<int>(4);
    }

    class Role
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

        public BsonDictionary<int, int> Nums = new BsonDictionary<int, int>();
        public BsonDictionary<int, string> Texts = new BsonDictionary<int, string>();
        public BsonDictionary<int, Item> Items = new BsonDictionary<int, Item>();
        public BsonDictionary<long, Friend> Friends = new BsonDictionary<long, Friend>();

        public BsonArray<int> ListInt = new BsonArray<int>(2);
        public BsonArray<string> ListStr = new BsonArray<string>(2);
        public BsonArray<Friend> ListFriend = new BsonArray<Friend>(2);

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

        public BsonDocument ToBsonDocument()
        {
            BsonDocument sb = new BsonDocument();
            sb.Add("RoleID", RoleID);
            sb.Add("RoleName", _RoleName);

            BsonDocument bsonFriends = new BsonDocument();
            Friends.Foreach(UpdateState.None | UpdateState.Set, (key, friend, state) =>
            {
                bsonFriends.Add(key.ToString(), friend.ToBsonDocument());
            });
            sb.Add("Friends", bsonFriends);

            BsonDocument bsonItems = new BsonDocument();
            Items.Foreach(UpdateState.None | UpdateState.Set, (key, item, state) =>
            {
                bsonItems.Add(key.ToString(), item.ToBsonDocument());
            });
            sb.Add("Items", bsonItems);

            BsonDocument bsonTexts = new BsonDocument();
            Texts.Foreach(UpdateState.None | UpdateState.Set, (key, text, state) =>
            {
                bsonTexts.Add(key.ToString(), text);
            });
            sb.Add("Texts", bsonTexts);

            BsonDocument bsonNums = new BsonDocument();
            Nums.Foreach(UpdateState.None | UpdateState.Set, (key, num, state) =>
            {
                bsonNums.Add(key.ToString(), num);
            });
            sb.Add("Nums", bsonNums);

            BsonArray bsonListInt = new BsonArray();
            for (int i = 0; i < ListInt.Count; i++)
            {
                bsonListInt.Add(ListInt[i]);
            }
            sb.Add("ListInt", bsonListInt);

            BsonArray bsonListStr = new BsonArray();
            for (int i = 0; i < ListStr.Count; i++)
            {
                bsonListStr.Add(null == ListStr[i] ? BsonNull.Value : (BsonValue)ListStr[i]);
            }
            sb.Add("ListStr", bsonListStr);

            BsonArray bsonListFriend = new BsonArray();
            for (int i = 0; i < ListFriend.Count; i++)
            {
                bsonListFriend.Add(null == ListFriend[i] ? BsonNull.Value : (BsonValue)ListFriend[i].ToBsonDocument());
            }
            sb.Add("ListFriend", bsonListFriend);

            return sb;
        }

        public void Save(string path, MongoSql sql)
        {
            if (_RoleNameDirty)
            {
                sql.Set.Add($"{path}RoleName", _RoleName);
            }

            Friends.Foreach(UpdateState.None | UpdateState.Set, (key, friend, state) =>
            {
                if (state == UpdateState.Set)
                {
                    sql.Set.Add($"{path}Friends.{key}", friend.ToBsonDocument());
                }
                else if (friend.DataDirty)
                {
                    friend.Save($"{path}Friends.{key}.", sql);
                }
            });

            Items.Foreach(UpdateState.None | UpdateState.Set, (key, item, state) =>
            {
                if (state == UpdateState.Set)
                {
                    sql.Set.Add($"{path}Items.{key}", item.ToBsonDocument());
                }
                else if (item.DataDirty)
                {
                    item.Save($"{path}Items.{key}.", sql);
                }
            });

            Texts.Foreach(UpdateState.Set, (key, text, state) =>
            {
                sql.Set.Add($"{path}Texts.{key}", text);
            });

            Nums.Foreach(UpdateState.Set, (key, num, state) =>
            {
                sql.Set.Add($"{path}Nums.{key}", num);
            });

            for (int i = 0; i < ListInt.Count; i++)
            {
                if (ListInt.IsModifyed(i))
                {
                    sql.Set.Add($"{path}ListInt.{i}", ListInt[i]);
                }
            }

            for (int i = 0; i < ListStr.Count; i++)
            {
                if (ListStr.IsModifyed(i))
                {
                    sql.Set.Add($"{path}ListStr.{i}", ListInt[i]);
                }
            }

            for (int i = 0; i < ListFriend.Count; i++)
            {
                Friend friend = ListFriend[i];
                if (ListFriend.IsModifyed(i))
                {
                    sql.Set.Add($"{path}ListFriend.{i}", friend.ToBsonDocument());
                }
                else
                {
                    if (null != friend && friend.DataDirty)
                    {
                        friend.Save($"{path}ListFriend.{i}.", sql);
                    }
                }
            }


            /// Unset
            Friends.Foreach(UpdateState.Unset, (key, friend, state) =>
            {
                sql.Unset.Add($"{path}Friends.{key}", 1);
            });

            Items.Foreach(UpdateState.Unset, (key, item, state) =>
            {
                sql.Unset.Add($"{path}Items.{key}", 1);
            });

            Texts.Foreach(UpdateState.Unset, (key, text, state) =>
            {
                sql.Unset.Add($"{path}Texts.{key}", 1);
            });

            Nums.Foreach(UpdateState.Unset, (key, num, state) =>
            {
                sql.Unset.Add($"{path}Nums.{key}", 1);
            });
        }
    }
}
