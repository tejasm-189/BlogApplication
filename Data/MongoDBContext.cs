using BlogApplication.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;

namespace BlogApplication.Services
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<BlogPost> BlogPosts => _database.GetCollection<BlogPost>("BlogPosts");
    }
}