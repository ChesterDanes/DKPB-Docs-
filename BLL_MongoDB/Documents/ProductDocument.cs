using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ProductDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("image")]
    public string Image { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("groupId")]
    public int? GroupId { get; set; }

    [BsonElement("groupName")]
    public string GroupName { get; set; }

    [BsonElement("cartItems")]
    public List<CartItemDocument> CartItems { get; set; } = new();
}

public class CartItemDocument
{
    [BsonElement("userId")]
    public int UserId { get; set; }

    [BsonElement("amount")]
    public int Amount { get; set; }
}
