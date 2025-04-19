using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BLL_MongoDb.Documents
{
    public class ProductGroupDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }

        [BsonElement("ID")]
        public int ID { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("ParentId")]
        public int? ParentId { get; set; }

        [BsonElement("Path")]
        public string Path { get; set; }
    }
}
