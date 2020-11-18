using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Test
{
    public class UnitMongo
    {
        static IMongoDatabase Database;

        static UnitMongo()
        {
            MongoClient client = new MongoClient("mongodb://localhost");
            Database = client.GetDatabase("Test");
        }

        [SetUp]
        public void Setup()
        {
            Database.DropCollection("Test");
        }

        public IMongoCollection<T> GetTestCollection<T>()
        {
            return Database.GetCollection<T>("Test");
        }

        [Test]
        public void TestBsonList()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { };
            role.ListInt.Add(0);
            role.ListInt.Add(1);
            role.ListStr.Add("A");
            role.ListStr.Add("B");
            role.ListFriend.Add(new Friend { RoleID = 1, RoleName = "friend1" });
            role.ListFriend.Add(new Friend { RoleID = 2, RoleName = "friend2" });
            collection.InsertOne((BsonDocument)role);

            var doc = collection.Find(new BsonDocument()).First();
            Role roleActual = (Role)doc;

            Assert.AreEqual(0, roleActual.ListInt[0]);
            Assert.AreEqual(1, roleActual.ListInt[1]);
            Assert.AreEqual("A", roleActual.ListStr[0]);
            Assert.AreEqual("B", roleActual.ListStr[1]);
            Assert.AreEqual(1, roleActual.ListFriend[0].RoleID);
            Assert.AreEqual("friend1", roleActual.ListFriend[0].RoleName);
            Assert.AreEqual(2, roleActual.ListFriend[1].RoleID);
            Assert.AreEqual("friend2", roleActual.ListFriend[1].RoleName);
        }

        [Test]
        public void TestBsonList_RemoveAt_Middle()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.RemoveAt(1);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(new BsonDocument {
                {
                    "$set",
                    new BsonDocument {
                        { "ListInt.3.PrevKey", 1 }
                    }
                },
                {
                    "$unset",
                    new BsonDocument {
                        { "ListInt.2", 1 }
                    }
                }
            }, bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(10, roleActual.ListInt[0]);
            Assert.AreEqual(12, roleActual.ListInt[1]);
        }

        [Test]
        public void TestBsonList_RemoveAt_First()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.RemoveAt(0);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(new BsonDocument {
                {
                    "$set",
                    new BsonDocument {
                        { "ListInt.2.PrevKey", 0 }
                    }
                },
                {
                    "$unset",
                    new BsonDocument {
                        { "ListInt.1", 1 }
                    }
                }
            }, bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(11, roleActual.ListInt[0]);
            Assert.AreEqual(12, roleActual.ListInt[1]);
        }

        [Test]
        public void TestBsonList_RemoveAt_Last()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.RemoveAt(2);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(new BsonDocument {
                {
                    "$unset",
                    new BsonDocument {
                        { "ListInt.3", 1 }
                    }
                }
            }, bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(10, roleActual.ListInt[0]);
            Assert.AreEqual(11, roleActual.ListInt[1]);
        }

        [Test]
        public void TestBsonList_Remove_Middle()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.Remove(11);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(new BsonDocument {
                {
                    "$set",
                    new BsonDocument {
                        { "ListInt.3.PrevKey", 1 }
                    }
                },
                {
                    "$unset",
                    new BsonDocument {
                        { "ListInt.2", 1 }
                    }
                }
            }, bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(10, roleActual.ListInt[0]);
            Assert.AreEqual(12, roleActual.ListInt[1]);
        }

        [Test]
        public void TestBsonList_Remove_First()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.Remove(10);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(new BsonDocument {
                {
                    "$set",
                    new BsonDocument {
                        { "ListInt.2.PrevKey", 0 }
                    }
                },
                {
                    "$unset",
                    new BsonDocument {
                        { "ListInt.1", 1 }
                    }
                }
            }, bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(11, roleActual.ListInt[0]);
            Assert.AreEqual(12, roleActual.ListInt[1]);
        }

        [Test]
        public void TestBsonList_Remove_Last()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.Remove(12);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(new BsonDocument {
                {
                    "$unset",
                    new BsonDocument {
                        { "ListInt.3", 1 }
                    }
                }
            }, bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(10, roleActual.ListInt[0]);
            Assert.AreEqual(11, roleActual.ListInt[1]);
        }

        [Test]
        public void TestBsonList_InsertFirst()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.Insert(0, 9);
            role.Save(null, context);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(9, roleActual.ListInt[0]);
            Assert.AreEqual(10, roleActual.ListInt[1]);
            Assert.AreEqual(11, roleActual.ListInt[2]);
            Assert.AreEqual(12, roleActual.ListInt[3]);
        }

        [Test]
        public void TestBsonList_InsertMiddle()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.Insert(1, 13);
            role.Save(null, context);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(10, roleActual.ListInt[0]);
            Assert.AreEqual(13, roleActual.ListInt[1]);
            Assert.AreEqual(11, roleActual.ListInt[2]);
            Assert.AreEqual(12, roleActual.ListInt[3]);
        }

        [Test]
        public void TestBsonList_AddLast()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.Add(13);
            role.Save(null, context);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(10, roleActual.ListInt[0]);
            Assert.AreEqual(11, roleActual.ListInt[1]);
            Assert.AreEqual(12, roleActual.ListInt[2]);
            Assert.AreEqual(13, roleActual.ListInt[3]);
        }

        [Test]
        public void TestBsonList_IndexSet()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt[1] = 21;
            role.Save(null, context);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(10, roleActual.ListInt[0]);
            Assert.AreEqual(21, roleActual.ListInt[1]);
            Assert.AreEqual(12, roleActual.ListInt[2]);
        }

        [Test]
        public void TestBsonListClear()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.ListInt.Add(10);
            role.ListInt.Add(11);
            role.ListInt.Add(12);
            role.ClearDirty();

            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListInt.Clear();
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(BsonDocument.Parse("{'$unset': {'ListInt': 1}}"), bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, context.Build());

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(0, roleActual.ListInt.Count);
        }

        [Test]
        public void TestBsonListAddNull()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.ListFriend.Add(null);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(BsonDocument.Parse("{$set:{'ListFriend.1': {'PrevKey':0, 'Value':null}}}"), bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, bsonUpdate);

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(null, roleActual.ListFriend[0]);
        }

        [Test]
        public void TestBsonDictAdd()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.Texts.Add(11, "ABC");
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Texts.11': 'ABC'}}"), bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, bsonUpdate);

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual("ABC", roleActual.Texts[11]);
        }

        [Test]
        public void TestBsonDictIndexSet()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.Texts[11] = "abc";
            role.ClearDirty();
            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.Texts[11] = "ABC";
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Texts.11': 'ABC'}}"), bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, bsonUpdate);

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual("ABC", roleActual.Texts[11]);
        }

        [Test]
        public void TestBsonDictRemove()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            role.Texts[11] = "abc";
            role.ClearDirty();
            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.Texts.Remove(11);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(BsonDocument.Parse("{$unset:{'Texts': 1}}"), bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, bsonUpdate);

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(0, roleActual.Texts.Count);
        }

        [Test]
        public void TestBsonDictSetNull()
        {
            var collection = GetTestCollection<BsonDocument>();

            Role role = new Role { RoleID = 1 };
            collection.InsertOne((BsonDocument)role);

            MongoContext context = new MongoContext();
            role.Friends.Add(11, null);
            role.Save(null, context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Friends.11': null}}"), bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, bsonUpdate);

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(null, roleActual.Friends[11]);
        }
    }
}
