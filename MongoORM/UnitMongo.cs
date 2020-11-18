using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoORM;
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

            UpdateContext context = new UpdateContext();
            role.ListInt.RemoveAt(1);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.RemoveAt(0);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.RemoveAt(2);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.Remove(11);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.Remove(10);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.Remove(12);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.Insert(0, 9);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.Insert(1, 13);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.Add(13);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt[1] = 21;
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListInt.Clear();
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.ListFriend.Add(null);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.Texts.Add(11, "ABC");
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.Texts[11] = "ABC";
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.Texts.Remove(11);
            role.Update(context);

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

            UpdateContext context = new UpdateContext();
            role.Friends.Add(11, null);
            role.Update(context);

            var bsonUpdate = context.Build();
            Assert.AreEqual(BsonDocument.Parse("{$set:{'Friends.11': null}}"), bsonUpdate);

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, bsonUpdate);

            var actual = collection.Find(new BsonDocument()).First();
            var roleActual = (Role)actual;

            Assert.AreEqual(null, roleActual.Friends[11]);
        }

        [Test]
        public void TestBulkWriteUpdate()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.BulkWrite(new WriteModel<BsonDocument>[] {
                new InsertOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "a"}, { "Value", 1 }, { "Text", "A" }, { "IntArray", new BsonArray(new int[] { 0, 1, 2 }) } }
                ),
                new InsertOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "b"}, { "Value", 2 }, { "Text", "B" }, { "IntArray", new BsonArray(new int[] { 10, 11, 12 }) } }
                )
            });

            collection.BulkWrite(new WriteModel<BsonDocument>[] {
                new UpdateOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "a" } }, new BsonDocument{ { "$set", new BsonDocument { { "Value", 11 }, { "Text", "AA" }, { "IntArray.0", 100 } } } }
                    ),
                new UpdateOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "b" } }, new BsonDocument{ { "$set", new BsonDocument { { "Value", 22 }, { "Text", "BB" }, { "IntArray.0", 110 } } } }
                    )
            });

            var doces = collection.Find(new BsonDocument()).ToList();
            BsonDocument doc1 = doces[0];
            BsonDocument doc2 = doces[1];

            Assert.AreEqual("a", (string)doc1["Name"]);
            Assert.AreEqual(11, (int)doc1["Value"]);
            Assert.AreEqual("AA", (string)doc1["Text"]);
            Assert.AreEqual(100, (int)doc1["IntArray"].AsBsonArray[0]);
            Assert.AreEqual(1, (int)doc1["IntArray"].AsBsonArray[1]);
            Assert.AreEqual(2, (int)doc1["IntArray"].AsBsonArray[2]);

            Assert.AreEqual("b", (string)doc2["Name"]);
            Assert.AreEqual(22, (int)doc2["Value"]);
            Assert.AreEqual("BB", (string)doc2["Text"]);
            Assert.AreEqual(110, (int)doc2["IntArray"].AsBsonArray[0]);
            Assert.AreEqual(11, (int)doc2["IntArray"].AsBsonArray[1]);
            Assert.AreEqual(12, (int)doc2["IntArray"].AsBsonArray[2]);
        }

        public void TestInsertMany()
        {
            var collection = GetTestCollection<BsonDocument>();

            Dictionary<long, Role> Roles = new Dictionary<long, Role>();
            for (int i = 0; i < 100; i++)
            {
                var role = new Role { RoleID = i, RoleName = i.ToString() };
                Roles.Add(i, role);
            }

            BulkWriteContext context = new BulkWriteContext();
            foreach (var pair in Roles)
            {
                context.Insert((BsonDocument)pair.Value);
            }

            context.Execute(collection);
        }

        [Test]
        public void TestBulkWriteRoles()
        {
            var collection = GetTestCollection<BsonDocument>();

            Dictionary<long, Role> Roles = new Dictionary<long, Role>();
            for (int i = 0; i < 100; i++)
            {
                var role = new Role { RoleID = i, RoleName = i.ToString(), Inserted = true };
                for (int itemIndex = 0; itemIndex < 50; itemIndex++)
                    role.Items.Add(itemIndex, new Item { ItemID = itemIndex, ItemUID = itemIndex });
                Roles.Add(i, role);
            }

            UpdateContext update = new UpdateContext();
            BulkWriteContext context = new BulkWriteContext();
            foreach (var pair in Roles)
            {
                if (pair.Value.Inserted)
                {
                    context.Insert((BsonDocument)pair.Value);
                }
                else
                {
                    update.Clear();
                    pair.Value.Update(update);
                    context.Update(new BsonDocument { { "_id", pair.Key } }, update.Build());
                }
                pair.Value.ClearDirty();
                pair.Value.Inserted = false;
            }
            context.Execute(collection);
            update.Clear();
            context.Clear();

            for (int i = 0; i < 100; i++)
            {
                Roles[i].RoleName += i;
                Roles[i].Items.Add(51, new Item { ItemID = 51, ItemUID = 51 });
            }
            Roles.Add(100, new Role { RoleID = 100, Inserted = true });

            foreach (var pair in Roles)
            {
                if (pair.Value.Inserted)
                {
                    context.Insert((BsonDocument)pair.Value);
                }
                else
                {
                    update.Clear();
                    pair.Value.Update(update);
                    context.Update(new BsonDocument { { "_id", pair.Key } }, update.Build());
                }
                pair.Value.ClearDirty();
                pair.Value.Inserted = false;
            }
            context.Execute(collection);
            update.Clear();
            context.Clear();
        }
    }

    public partial class Role
    {
        public bool Inserted;
    }
}
