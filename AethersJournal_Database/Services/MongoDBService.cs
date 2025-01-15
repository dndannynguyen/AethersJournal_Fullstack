using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDBService
{
    private readonly IMongoDatabase _database;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)

    {
        var mongoDbConnectionString = Configuration["MONGODB_CONNECTION_STRING"]; 
        var client = new MongoClient(mongoDbConnectionString);
        _database = client.GetDatabase("AethersJournal");
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<UserStatus> UserStatus => _database.GetCollection<UserStatus>("userStatus");
    public IMongoCollection<JournalEntry> JournalEntries => _database.GetCollection<JournalEntry>("journalEntries");
    public IMongoCollection<Chat> Chat => _database.GetCollection<Chat>("chat");
    public IMongoCollection<Session> Session => _database.GetCollection<Session>("session");
}
