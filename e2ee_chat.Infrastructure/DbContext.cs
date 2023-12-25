using e2ee_chat.Infrastructure.Schemas;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace e2ee_chat.Infrastructure;

public class DbContext
{
    private readonly IMongoDatabase _db;


    public DbContext(string dbName)
    {
        var appSettings = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "e2ee_chat.Infrastructure", "appsettings.json");
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(appSettings)
            .Build();
        
        var settings = MongoClientSettings.FromConnectionString(config.GetConnectionString("MongoDB"));
        var client = new MongoClient(settings);
        _db = client.GetDatabase(dbName);
    }

    public IMongoCollection<User> Users => _db.GetCollection<User>("User");
    public IMongoCollection<UserKeys> UserKeys => _db.GetCollection<UserKeys>("SecureUserKeys");
}