using System;
namespace backend.Configuration;


public class MongoDbSettings
{
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UsersCollectionName { get; set; } = string.Empty;
        public string DevicesCollectionName {  get; set; } = string.Empty;
}
