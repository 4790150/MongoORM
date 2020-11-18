using MongoDB.Bson;
using NUnit.Framework;

namespace Test
{
    public class UnitTest
    {
        MongoContext context = new MongoContext();

        Role CreateRole()
        {
            Role role = new Role();
            return role;
        }

        [SetUp]
        public void Setup()
        {
            context.Clear();
        }

        [Test]
        public void SetProperty()
        {
            Role role = CreateRole();
            role.RoleName = "test";

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'RoleName':'test'}}"), context.Build());

            role.ClearDirty();
            context.Clear();

            role.RoleName = null;
            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'RoleName':1}}"), context.Build());
        }

        [Test]
        public void SetSubDocumentProperty()
        {
            Role role = CreateRole();
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "test2" });
            role.Friends.ClearDirty();

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Friends.2.RoleName':'test2'}}"), context.Build());
        }

        [Test]
        public void SetSubDocumentClass()
        {
            Role role = CreateRole();
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "test2" });

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Friends.2':{'_id':2,'RoleName':'test2'}}}"), context.Build());
        }

        [Test]
        public void SetSubDocumentString()
        {
            Role role = CreateRole();
            role.Texts.Add(2, "test2");

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Texts.2':'test2'}}"), context.Build());

            role.ClearDirty();
        }

        [Test]
        public void SetSubDocumentPrimary()
        {
            Role role = CreateRole();
            role.Nums.Add(2, 2);

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Nums.2':2}}"), context.Build());
        }

        [Test]
        public void SetPrimaryMulti()
        {
            Role role = CreateRole();
            role.Nums.Add(2, 2);
            role.Nums.Add(3, 3);

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Nums.2':2,'Nums.3':3}}"), context.Build());
        }

        [Test]
        public void UnsetDocumentClass()
        {
            Role role = CreateRole();
            role.Friends.Add(1, null);
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "test2" });
            role.ClearDirty();
            role.Friends.Remove(2);

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'Friends.2':1}}"), context.Build());
        }

        [Test]
        public void UnsetDocumentString()
        {
            Role role = CreateRole();
            role.Texts.Add(1, "test1");
            role.Texts.Add(2, "test2");
            role.ClearDirty();
            role.Texts.Remove(2);

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'Texts.2':1}}"), context.Build());
        }

        [Test]
        public void UnsetDocumentPrimary()
        {
            Role role = CreateRole();
            role.Nums.Add(1, 1);
            role.Nums.Add(2, 2);
            role.ClearDirty();
            role.Nums.Remove(2);

            role.Save(null, context);
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'Nums.2':1}}"), context.Build());
        }

        [Test]
        public void Document()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";

            role.Save(null, context);
            var actual = (BsonValue)role;
            var expected = BsonDocument.Parse("{'_id':NumberLong(1),'RoleName':'testName'}");
            Assert.AreEqual(expected, (BsonDocument)(BsonValue)role);
        }

        [Test]
        public void SubDocumentClass()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";
            role.Friends.Add(2, new Friend { RoleID = 2, RoleName = "friendName" });

            Assert.AreEqual(BsonDocument.Parse("{'_id':NumberLong(1),'RoleName':'testName','Friends':{'2':{'_id':2,'RoleName':'friendName'}}}"), (BsonValue)role);
        }

        [Test]
        public void SubDocumentString()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";
            role.Texts.Add(2, "friendName");

            Assert.AreEqual(BsonDocument.Parse("{'_id':NumberLong(1),'RoleName':'testName','Texts':{'2':'friendName'}}"), (BsonValue)role);
        }

        [Test]
        public void SubDocumentPrimary()
        {
            Role role = CreateRole();
            role.RoleID = 1;
            role.RoleName = "testName";
            role.Nums.Add(2, 2);

            Assert.AreEqual(BsonDocument.Parse("{'_id':NumberLong(1),'RoleName':'testName','Nums':{'2':2}}"), (BsonValue)role);
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
        }
    }
}