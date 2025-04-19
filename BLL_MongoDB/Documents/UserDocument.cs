using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BLL_MongoDb.Documents
{
    public class UserDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; }

        [BsonElement("ID")]
        public int ID { get; set; }

        [BsonElement("Login")]
        public string Login { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        [BsonElement("GroupId")]
        public int? GroupId { get; set; }
    }
}
