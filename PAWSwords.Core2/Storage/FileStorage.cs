using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PAWSwords.Core.Storage
{
    public class FileStorage
    {
        private readonly string StorageDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PAWSwords");
        private readonly string PasswordsFile = "pawswords";
        private readonly JsonSerializer _serializer;

        private readonly string _fullStorageFilePath;

        public FileStorage()
        {
            _serializer = new JsonSerializer();
            _fullStorageFilePath = Path.Combine(StorageDirectory, PasswordsFile);
        }

        public async Task Store(StorageModel model)
        {
            Directory.CreateDirectory(StorageDirectory);

            using (var fileStream = File.OpenWrite(_fullStorageFilePath))
            using (var bsonWriter = new BsonDataWriter(fileStream))
            {
                _serializer.Serialize(bsonWriter, model);
            }
        }

        public async Task<StorageModel> Read()
        {
            using (var fileStream = File.OpenRead(_fullStorageFilePath))
            using (var reader = new BsonDataReader(fileStream))
            {
                return _serializer.Deserialize<StorageModel>(reader);
            }
        }
    }
}
