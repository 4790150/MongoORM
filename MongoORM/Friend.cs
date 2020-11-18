using MongoDB.Bson;
using MongoORM;
using System;

namespace Test
{
    public partial class Friend
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
                if (_RoleNameDirty) return true;
                return false;
            }
        }

        public void ClearDirty()
        {
            _RoleNameDirty = false;
        }

        public static explicit operator BsonValue(Friend item)
        {
            if (null == item)
                return BsonNull.Value;

            return (BsonDocument)item;
        }

        public static explicit operator BsonDocument(Friend item)
        {
            if (null == item)
                return null;

            BsonDocument document = new BsonDocument();
            document.Add("_id", item.RoleID);
            if (default(string) != item._RoleName)
                document.Add("RoleName", item._RoleName);
            return document;
        }

        public static explicit operator Friend(BsonValue bsonValue)
        {
            if (bsonValue.IsBsonNull)
                return null;

            return (Friend)bsonValue.AsBsonDocument;
        }

        public static explicit operator Friend(BsonDocument document)
        {
            Friend item = new Friend();
            foreach (var field in document)
            {
                switch(field.Name)
                {
                    case "_id": item.RoleID = (long)field.Value; break;
                    case "RoleName": item._RoleName = (string)field.Value; break;
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
        }
    }
}
