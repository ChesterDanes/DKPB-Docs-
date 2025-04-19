using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace BLL_MongoDb.Documents
{
    public class OrderPositionDocument
    {
        [BsonElement("ProductName")]
        public string ProductName { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }

        [BsonElement("Amount")]
        public int Amount { get; set; }

        [BsonElement("Value")]
        public decimal Value { get; set; }
    }
}