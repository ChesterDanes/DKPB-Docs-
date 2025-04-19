using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using BLL_MongoDb.Documents;

namespace BLL_MongoDb.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase(configuration.GetSection("MongoDbSettings:DatabaseName").Value);
        }

        public IMongoCollection<ProductDocument> Products => _database.GetCollection<ProductDocument>("Products");
        public IMongoCollection<UserDocument> Users => _database.GetCollection<UserDocument>("Users");
        public IMongoCollection<ProductGroupDocument> ProductGroups => _database.GetCollection<ProductGroupDocument>("ProductGroups");
        public IMongoCollection<OrderDocument> Orders => _database.GetCollection<OrderDocument>("Orders");
    }
}