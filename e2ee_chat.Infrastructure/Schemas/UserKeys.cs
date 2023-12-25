using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace e2ee_chat.Infrastructure.Schemas;

public class UserKeys
{
    [BsonId] public ObjectId Id { get; set; }
    [BsonElement("UserId")] public string UserId { get; set; }
    [BsonElement("IV")] public byte[] IV { get; set; }
    [BsonElement("PasswordSalt")] public byte[] Passwordsalt { get; set; }
}