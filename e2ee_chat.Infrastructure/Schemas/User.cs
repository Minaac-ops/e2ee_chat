using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace e2ee_chat.Infrastructure.Schemas;

public class User
{
    [BsonId] public ObjectId Id { get; set; }
    [BsonElement("Email")] public string Email { get; set; }
    [BsonElement("Username")] public string Username { get; set; }
    [BsonElement("EncryptedRandom")] public byte[] EncryptedRandom { get; set; }
    [BsonElement("RandomString")] public string RandomString { get; set; }
    
}