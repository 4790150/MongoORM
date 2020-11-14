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
            Database = client.GetDatabase("test");
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
        public void TestChar()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':65}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual('A', (char)document["TestValue"]);
            Assert.AreEqual(65, (int)document["TestValue"]);
        }

        [Test]
        public void TestString()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':'Abc'}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual("Abc", (string)document["TestValue"]);
        }

        [Test]
        public void TestBoolean()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':true}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(true, (bool)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':false}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(false, (bool)document["TestValue"]);
        }

        [Test]
        public void TestByte()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{byte.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(byte.MaxValue, (byte)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{byte.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(byte.MinValue, (byte)document["TestValue"]);
        }

        [Test]
        public void TestSByte()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{sbyte.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(sbyte.MaxValue, (byte)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{sbyte.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(sbyte.MinValue, (sbyte)document["TestValue"]);
        }

        [Test]
        public void TestShort()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{short.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(short.MaxValue, (short)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{short.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(short.MinValue, (short)document["TestValue"]);
        }

        [Test]
        public void TestUShort()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{ushort.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(ushort.MaxValue, (ushort)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{ushort.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(ushort.MinValue, (ushort)document["TestValue"]);
        }

        [Test]
        public void TestInt()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{int.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(int.MaxValue, (int)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{int.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(int.MinValue, (int)document["TestValue"]);
        }

        [Test]
        public void TestUInt()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{uint.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(uint.MaxValue, (uint)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{uint.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(uint.MinValue, (uint)document["TestValue"]);
        }

        [Test]
        public void TestLong()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':NumberLong({long.MaxValue})}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(long.MaxValue, (long)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':NumberLong({long.MinValue})}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(long.MinValue, (long)document["TestValue"]);
        }

        [Test]
        public void TestULong()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':NumberLong({ulong.MaxValue})}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(ulong.MaxValue, (ulong)document["TestValue"].AsInt64);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':NumberLong({int.MinValue})}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(ulong.MinValue, (ulong)document["TestValue"].AsInt64);
        }

        [Test]
        public void TestFloat()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{float.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(float.MaxValue, (float)document["TestValue"].AsDouble);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{float.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(float.MinValue, (float)document["TestValue"].AsDouble);
        }

        [Test]
        public void TestDouble()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{double.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(double.MaxValue, (double)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{double.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(double.MinValue, (double)document["TestValue"]);
        }

        [Test]
        public void TestDecimal()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{decimal.MaxValue}}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(decimal.MaxValue, (decimal)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':{decimal.MinValue}}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(decimal.MinValue, (decimal)document["TestValue"]);
        }

        [Test]
        public void TestDateTime()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':ISODate('{BsonValue.Create(DateTime.MaxValue)}')}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(DateTime.MaxValue, (DateTime)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':ISODate('{BsonValue.Create(DateTime.MinValue)}')}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(DateTime.MinValue, (DateTime)document["TestValue"]);
        }

        [Test]
        public void TestTimespan()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':'{TimeSpan.MaxValue}'}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(TimeSpan.MaxValue.ToString(), (string)document["TestValue"]);

            Database.DropCollection("Test");

            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':'{TimeSpan.MinValue}'}}"));
            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(TimeSpan.MinValue.ToString(), (string)document["TestValue"]);
        }

        [Test]
        public void TestGuid()
        {
            var guid = Guid.NewGuid();
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':'{guid}'}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(guid, Guid.Parse((string)document["TestValue"]));
        }

        [Test]
        public void TestByteArray()
        {
            byte[] arr = new byte[] { 0, 1, 2, 3, 4, 5 };
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(BsonDocument.Parse($"{{'TestValue':[0,1,2,3,4,5]}}"));
            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreNotEqual(arr, document["TestValue"]);
        }

        [Test]
        public void TestEntity()
        {
            Database.DropCollection("Entity");
            var collection = Database.GetCollection<Entity>("Entity");
            collection.InsertOne(new Entity());
        }

        [Test]
        public void TestSerializer()
        {
            BsonSerializer.RegisterSerializer(new DateSerializer());

            Database.DropCollection("Entity");
            var collection = Database.GetCollection<Entity>("Entity");
            collection.InsertOne(new Entity());
            var document = collection.Find<Entity>(new BsonDocument()).First();
        }

        class Document
        {
            public string name;
        }

        [Test]
        public void TestProfile()
        {
            Stopwatch watch = new Stopwatch();
            Database.DropCollection("Document2");
            Database.DropCollection("Document");

            var collection2 = Database.GetCollection<BsonDocument>("Document2");
            var doc2 = new BsonDocument { { "name", "niehong" } };
            watch.Start();
            for (int i = 0; i < 10000; i++)
                collection2.InsertOne(new BsonDocument { { "name", "niehong" } });
            watch.Stop();
            var time2 = watch.ElapsedMilliseconds;

            watch.Reset();

            var collection1 = Database.GetCollection<Document>("Document");
            var doc1 = new Document { name = "niehong" };
            watch.Start();
            for (int i = 0; i < 10000; i++)
                collection1.InsertOne(new Document { name = "niehong" });
            watch.Stop();
            var time1 = watch.ElapsedMilliseconds;

            Assert.IsTrue(time1 < time2);
        }

        [Test]
        public void TestUpdateBsonDocument()
        {
            Database.DropCollection("Document");

            var collection = Database.GetCollection<BsonDocument>("Document");
            collection.InsertOne(new BsonDocument { { "_id", 1 }, { "Name", "niehong" } });

            collection.UpdateOne(new BsonDocument { { "_id", 1 } }, new BsonDocument { { "$set", new BsonDocument { { "Name", "NieHong" } } } });
            var document = collection.Find(new BsonDocument { { "_id", 1 } }).First();

            Assert.AreEqual("NieHong", (string)document["Name"]);
        }

        [Test]
        public void TestUpdateProfile()
        {
            Stopwatch watch = new Stopwatch();
            Database.DropCollection("Document");

            var collection = Database.GetCollection<BsonDocument>("Document");
            collection.InsertOne(new BsonDocument { { "_id", 1 }, { "Name", "niehong" } });

            watch.Start();
            for (int i = 0; i < 100000; i++)
                collection.UpdateOne(new BsonDocument { { "_id", 1 } }, new BsonDocument { { "$set", new BsonDocument { { "Name", "NieHong" } } } });
            watch.Stop();
            long time1 = watch.ElapsedTicks;

            watch.Reset();

            watch.Start();
            for (int i = 0; i < 100000; i++)
                collection.UpdateOne(new BsonDocument { { "_id", 1 } }, Builders<BsonDocument>.Update.Set("Name", "NieHong"));
            watch.Stop();
            long time2 = watch.ElapsedTicks;

            Assert.IsTrue(time1 < time2);
        }

        [Test]
        public void TestPush()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(new BsonDocument { { "IntArray", new BsonArray(new int[] { 0 }) } });

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$push",
                    new BsonDocument {
                        { "IntArray", 1 }
                    }
                }
            });

            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(0, (int)document["IntArray"][0]);
            Assert.AreEqual(1, (int)document["IntArray"][1]);
        }

        [Test]
        public void TestPush_Multi()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(new BsonDocument { { "IntArray", new BsonArray(new int[] { 0 }) } });

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$push",
                    new BsonDocument {
                        { "IntArray",
                            new BsonDocument {
                                { "$each",
                                    new BsonArray{ 1,2 }
                                }
                            }
                        }
                    }
                }
            });

            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(0, (int)document["IntArray"][0]);
            Assert.AreEqual(1, (int)document["IntArray"][1]);
            Assert.AreEqual(2, (int)document["IntArray"][2]);
        }

        [Test]
        public void TestPopMinus1()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(new BsonDocument { { "IntArray", new BsonArray(new int[] { 0, 1, 2 }) } });

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$pop",
                    new BsonDocument {
                        { "IntArray", -1 }
                    }
                }
            });

            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(1, (int)document["IntArray"][0]);
            Assert.AreEqual(2, (int)document["IntArray"][1]);
            Assert.AreEqual(2, document["IntArray"].AsBsonArray.Count);
        }

        [Test]
        public void TestPopPlus1()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(new BsonDocument { { "IntArray", new BsonArray(new int[] { 0, 1, 2 }) } });

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$pop",
                    new BsonDocument {
                        { "IntArray", 1 }
                    }
                }
            });

            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(0, (int)document["IntArray"][0]);
            Assert.AreEqual(1, (int)document["IntArray"][1]);
            Assert.AreEqual(2, document["IntArray"].AsBsonArray.Count);
        }

        [Test]
        public void TestArraySetAfterPop()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(new BsonDocument { { "IntArray", new BsonArray(new int[] { 0, 1, 2 }) }, { "StringArray", new BsonArray(new string[] { "A", "B", "C" }) } });

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$pop",
                    new BsonDocument {
                        { "IntArray", 1 }
                    }
                },
                { "$set",
                    new BsonDocument
                    {
                        { "StringArray.0", "a" }
                    }
                }
            });

            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(2, document["IntArray"].AsBsonArray.Count);
            Assert.AreEqual("a", (string)document["StringArray"][0]);
        }

        [Test]
        public void TestAddToSet()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(new BsonDocument { { "IntArray", new BsonArray(new int[] { 0, 1, 2 }) } });

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$addToSet",
                    new BsonDocument {
                        { "IntArray", 1 }
                    }
                }
            });

            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(0, (int)document["IntArray"][0]);
            Assert.AreEqual(1, (int)document["IntArray"][1]);
            Assert.AreEqual(2, (int)document["IntArray"][2]);
            Assert.AreEqual(3, document["IntArray"].AsBsonArray.Count);

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$addToSet",
                    new BsonDocument {
                        { "IntArray", 3 }
                    }
                }
            });

            document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(0, (int)document["IntArray"][0]);
            Assert.AreEqual(1, (int)document["IntArray"][1]);
            Assert.AreEqual(2, (int)document["IntArray"][2]);
            Assert.AreEqual(3, (int)document["IntArray"][3]);
            Assert.AreEqual(4, document["IntArray"].AsBsonArray.Count);
        }

        [Test]
        public void TestAddToSet_Multi()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertOne(new BsonDocument { { "IntArray", new BsonArray(new int[] { 0, 1, 2 }) } });

            collection.UpdateOne(new BsonDocument(), new BsonDocument {
                { "$addToSet",
                    new BsonDocument {
                        { "IntArray",
                            new BsonDocument{
                                { "$each",
                                    new BsonArray(new int[] { 4, 2, 3, 1, 0 })
                                }
                            }
                        }
                    }
                }
            });

            var document = collection.Find<BsonDocument>(new BsonDocument()).First();
            Assert.AreEqual(0, (int)document["IntArray"][0]);
            Assert.AreEqual(1, (int)document["IntArray"][1]);
            Assert.AreEqual(2, (int)document["IntArray"][2]);
            Assert.AreEqual(4, (int)document["IntArray"][3]);
            Assert.AreEqual(3, (int)document["IntArray"][4]);
            Assert.AreEqual(5, document["IntArray"].AsBsonArray.Count);
        }

        [Test]
        public void TestBulkWriteInsert()
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

            var doces = collection.Find(new BsonDocument()).ToList();
            BsonDocument doc1 = doces[0];
            BsonDocument doc2 = doces[1];

            Assert.AreEqual("a", (string)doc1["Name"]);
            Assert.AreEqual(1, (int)doc1["Value"]);
            Assert.AreEqual("A", (string)doc1["Text"]);
            Assert.AreEqual(0, (int)doc1["IntArray"].AsBsonArray[0]);
            Assert.AreEqual(1, (int)doc1["IntArray"].AsBsonArray[1]);
            Assert.AreEqual(2, (int)doc1["IntArray"].AsBsonArray[2]);

            Assert.AreEqual("b", (string)doc2["Name"]);
            Assert.AreEqual(2, (int)doc2["Value"]);
            Assert.AreEqual("B", (string)doc2["Text"]);
            Assert.AreEqual(10, (int)doc2["IntArray"].AsBsonArray[0]);
            Assert.AreEqual(11, (int)doc2["IntArray"].AsBsonArray[1]);
            Assert.AreEqual(12, (int)doc2["IntArray"].AsBsonArray[2]);
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

        [Test]
        public void TestBulkWriteConflict()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.BulkWrite(new WriteModel<BsonDocument>[] {
                new InsertOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "a"}, { "Value", 1 }, { "Text", "A" }, { "IntArray", new BsonArray(new int[] { 0, 1, 2 }) } }
                ),
            });

            collection.BulkWrite(new WriteModel<BsonDocument>[] {
                new UpdateOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "a" } }, new BsonDocument{ { "$set", new BsonDocument { { "Value", 11 }, { "Text", "AA" }, { "IntArray.0", 100 } } } }
                    ),
                new UpdateOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "a" } }, new BsonDocument{ { "$set", new BsonDocument { { "Value", 22 }, { "Text", "BB" } } }, { "$push", new BsonDocument { { "IntArray", 3 } } } }
                    )
            });

            var doc = collection.Find(new BsonDocument()).First();

            Assert.AreEqual("a", (string)doc["Name"]);
            Assert.AreEqual(22, (int)doc["Value"]);
            Assert.AreEqual("BB", (string)doc["Text"]);
            Assert.AreEqual(100, (int)doc["IntArray"].AsBsonArray[0]);
            Assert.AreEqual(1, (int)doc["IntArray"].AsBsonArray[1]);
            Assert.AreEqual(2, (int)doc["IntArray"].AsBsonArray[2]);
            Assert.AreEqual(3, (int)doc["IntArray"].AsBsonArray[3]);
        }

        [Test]
        public void TestBulkWriteProfile()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.BulkWrite(new WriteModel<BsonDocument>[] {
                new InsertOneModel<BsonDocument> (
                    new BsonDocument { { "Name", "a"}, { "Value", 1 } }
                ),
            });

            Stopwatch watch = new Stopwatch();

            watch.Start();
            for (int i = 0; i < 10000; i++)
            {
                collection.BulkWrite(new WriteModel<BsonDocument>[] {
                    new UpdateOneModel<BsonDocument> (
                        new BsonDocument { { "Name", "a" } }, new BsonDocument{ { "$set", new BsonDocument { { "Value", i } } } }
                        )
                });
            }
            watch.Stop();
            long time1 = watch.ElapsedMilliseconds;
            watch.Reset();

            watch.Start();
            for (int i = 0; i < 10000; i++)
            {
                collection.UpdateOne(new BsonDocument { { "Name", "a" } }, new BsonDocument{ { "$set", new BsonDocument { { "Value", i } } } });
            }
            watch.Stop();
            long time2 = watch.ElapsedMilliseconds;

            Assert.IsTrue(time1 > time2);
        }

        [Test]
        public void TestSubDocumentOrder()
        {
            var collection = GetTestCollection<BsonDocument>();
            collection.InsertMany(new BsonDocument[] {
                new BsonDocument { { "Value1", 1 }, { "Value2", 2 } },
            });

            collection.UpdateOne(new BsonDocument(), new BsonDocument { { "$set", new BsonDocument { { "Value1", 10 } } } });
            var doc = collection.Find(new BsonDocument()).First();

            Assert.AreEqual(10, (int)doc[1]);
            Assert.AreEqual(2, (int)doc[2]);
        }
    }


    class DateSerializer : IBsonSerializer<DateTime>
    {
        public Type ValueType => typeof(DateTime);

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            long ticks = context.Reader.ReadInt64();
            return new DateTime(ticks);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            context.Writer.WriteInt64(value.Ticks);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            context.Writer.WriteInt64(((DateTime)value).Ticks);
        }

        DateTime IBsonSerializer<DateTime>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            long ticks = context.Reader.ReadInt64();
            return new DateTime(ticks);
        }
    }

    class Entity
    {
        public ObjectId Id;
        public char charValue = 'A';
        public string stringValue = "ABC中国人";
        public bool boolValueMax = true;
        public bool boolValueMin = false;
        public byte byteValueMax = byte.MaxValue;
        public byte byteValueMin = byte.MinValue;
        public sbyte sbyteValueMax = sbyte.MaxValue;
        public sbyte sbyteValueMin = sbyte.MinValue;
        public short shortValueMax = short.MaxValue;
        public short shortValueMin = short.MinValue;
        public ushort ushortValueMax = ushort.MaxValue;
        public ushort ushortValueMin = ushort.MinValue;
        public int intValueMax = int.MaxValue;
        public int intValueMin = int.MinValue;
        //public uint uintValueMax = uint.MaxValue;
        //public uint uintValueMin = uint.MinValue;
        public long longValueMax = long.MaxValue;
        public long longValueMin = long.MinValue;
        //public ulong ulongValueMax = ulong.MaxValue;
        //public ulong ulongValueMin = ulong.MinValue;
        public float floatValueMax = float.MaxValue;
        public float floatValueMin = float.MinValue;
        public double doubleValueMax = double.MaxValue;
        public double doubleValueMin = double.MinValue;
        public decimal decimalValueMax = decimal.MaxValue;
        public decimal decimalValueMin = decimal.MinValue;
        public DateTime DateTimeValueMax = DateTime.MaxValue;
        public DateTime DateTimeValueMin = DateTime.MinValue;
        public TimeSpan TimeSpanValueMax = TimeSpan.MaxValue;
        public TimeSpan TimeSpanValueMin = TimeSpan.MinValue;
        public Guid GuidValueMax = Guid.NewGuid();
        public byte[] byteArray = new byte[] { 0, 1, 2, 3, 4, 5 };
        public int[] intArray = new int[] { 0, 1, 2, 3, 4, 5 };
        public string[] stringArray = new string[] { "a", "b", "c" };
        public List<byte> byteList = new List<byte> { 0, 1, 2, 3, 4, 5 };
        public List<string> stringList = new List<string> { "a", "b", "c" };
        public Dictionary<string, string> dictIntString = new Dictionary<string, string> { { "0", "a" }, { "1", "b" } };
        public Dictionary<string, SubDocument> dictDocument = new Dictionary<string, SubDocument> { { "a", new SubDocument() } };
    }

    class SubDocument
    {
    }
}
