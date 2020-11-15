using MongoDB.Bson;
using System;
using Test;

namespace Test
{

    public class Friend
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
        public bool DataDirty
        {
            get
            {
                bool dirty = false;
                if (_RoleNameDirty) return true;
                return false;
            }
        }

        public void ClearState()
        {
            _RoleNameDirty = false;
        }

        public static explicit operator BsonValue(Friend item)
        {
            BsonDocument sb = new BsonDocument();
            sb.Add("RoleID", item.RoleID);
            sb.Add("RoleName", item._RoleName);
            return sb;
        }

        public static explicit operator Friend(BsonValue bsonValue)
        {
            BsonDocument document = bsonValue.AsBsonDocument;
            
            Friend item = new Friend();
            item.RoleID = (long)document["RoleID"];
            item._RoleName = (string)document["RoleName"];
            return item;
        }

        public void Save(string path, MongoSql sql)
        {
            if (_RoleNameDirty)
            {
                sql.Set.Add($"{path}RoleName", _RoleName);
            }
        }
    }
}
