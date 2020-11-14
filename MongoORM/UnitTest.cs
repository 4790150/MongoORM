using MongoDB.Bson;
using NUnit.Framework;

namespace Test
{
    public class UnitTest
    {
        MongoSql sql = new MongoSql();

        Role CreateRole()
        {
            Role role = new Role();
            return role;
        }

        [SetUp]
        public void Setup()
        {
            sql.Clear();
        }

        [Test]
        public void SetProperty()
        {
            Role role = CreateRole();
            role.RoleName = "test";

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'RoleName':'test'}}"), sql.ToSql());
        }

        [Test]
        public void SetSubDocumentProperty()
        {
            Role role = CreateRole();
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "test2" }, false);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Friends.2.RoleName':'test2'}}"), sql.ToSql());
        }

        [Test]
        public void SetDocumentClass()
        {
            Role role = CreateRole();
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "test2" }, true);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Friends.2':{'RoleID':2,'RoleName':'test2'}}}"), sql.ToSql());
        }

        [Test]
        public void SetDocumentString()
        {
            Role role = CreateRole();
            role.Texts.Add(2, "test2", true);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Texts.2':'test2'}}"), sql.ToSql());
        }

        [Test]
        public void SetDocumentPrimary()
        {
            Role role = CreateRole();
            role.Nums.Add(2, 2, true);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Nums.2':2}}"), sql.ToSql());
        }

        [Test]
        public void SetPrimaryMulti()
        {
            Role role = CreateRole();
            role.Nums.Add(2, 2, true);
            role.Nums.Add(3, 3, true);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Nums.2':2,'Nums.3':3}}"), sql.ToSql());
        }

        [Test]
        public void UnsetDocumentClass()
        {
            Role role = CreateRole();
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "test2" }, false);
            role.Friends.Remove(2);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'Friends.2':1}}"), sql.ToSql());
        }

        [Test]
        public void UnsetDocumentString()
        {
            Role role = CreateRole();
            role.Texts.Add(2, "test2", false);
            role.Texts.Remove(2);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'Texts.2':1}}"), sql.ToSql());
        }

        [Test]
        public void UnsetDocumentPrimary()
        {
            Role role = CreateRole();
            role.Nums.Add(2, 2, false);
            role.Nums.Remove(2);

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'Nums.2':1}}"), sql.ToSql());
        }

        [Test]
        public void Document()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";

            role.Save(null, sql);
            Assert.AreEqual(BsonDocument.Parse("{'RoleID':NumberLong(1),'RoleName':'testName','Friends':{},'Items':{},'Texts':{},'Nums':{},'ListInt':[0,0],'ListStr':[null,null],'ListFriend':[null,null]}"), role.ToBsonDocument());
        }

        [Test]
        public void SubDocumentClass()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "friendName" });

            Assert.AreEqual(BsonDocument.Parse("{'RoleID':NumberLong(1),'RoleName':'testName','Friends':{'2':{'RoleID':2,'RoleName':'friendName'}},'Items':{},'Texts':{},'Nums':{},'ListInt':[0,0],'ListStr':[null,null],'ListFriend':[null,null]}"), role.ToBsonDocument());
        }

        [Test]
        public void SubDocumentString()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";
            role.Texts.Add(2, "friendName");

            Assert.AreEqual(BsonDocument.Parse("{'RoleID':NumberLong(1),'RoleName':'testName','Friends':{},'Items':{},'Texts':{'2':'friendName'},'Nums':{},'ListInt':[0,0],'ListStr':[null,null],'ListFriend':[null,null]}"), role.ToBsonDocument());
        }

        [Test]
        public void SubDocumentPrimary()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";
            role.Nums.Add(2, 2);

            Assert.AreEqual(BsonDocument.Parse("{'RoleID':NumberLong(1),'RoleName':'testName','Friends':{},'Items':{},'Texts':{},'Nums':{'2':2},'ListInt':[0,0],'ListStr':[null,null],'ListFriend':[null,null]}"), role.ToBsonDocument());
        }

        [Test]
        public void DictDocumentCount()
        {
            BsonDictionary<int, string> dict = new BsonDictionary<int, string>();
            Assert.AreEqual(0, dict.Count);
            dict.Add(1, null);
            Assert.AreEqual(1, dict.Count);
            dict.Add(1, "");
            Assert.AreEqual(1, dict.Count);
            dict.Add(2, "");
            Assert.AreEqual(2, dict.Count);
            dict.Remove(1);
            Assert.AreEqual(1, dict.Count);
            dict.SetState(1, UpdateState.None);
            Assert.AreEqual(2, dict.Count);
            dict.SetState(1, UpdateState.Unset);
            Assert.AreEqual(1, dict.Count);
            dict.SetState(1, UpdateState.Set);
            Assert.AreEqual(2, dict.Count);
            dict.SetState(1, UpdateState.Unset);
            dict.ClearState();
            Assert.AreEqual(1, dict.Count);
        }
    }
}