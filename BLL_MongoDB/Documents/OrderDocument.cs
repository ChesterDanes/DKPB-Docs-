using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace BLL_MongoDb.Documents
{
    public class OrderDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }

        [BsonElement("ID")]
        public int ID { get; set; }

        [BsonElement("UserID")]
        public int UserID { get; set; }

        [BsonElement("Value")]
        public decimal Value { get; set; }

        [BsonElement("IsPaid")]
        public bool IsPaid { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }

        [BsonElement("Positions")]
        public List<OrderPositionDocument> Positions { get; set; }
    }
}