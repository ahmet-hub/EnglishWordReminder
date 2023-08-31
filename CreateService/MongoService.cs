using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace CreateService
{
    public class MongoService
    {
        private MongoClient Client { get; set; }
        private IMongoDatabase Database { get; set; }
        private IMongoCollection<BsonDocument> Collection { get; set; }
        public MongoService()
        {
            Client = new MongoClient("mongodb+srv://ahmetyardimci:WLdiXCeoeqVqdOAB@cluster0.lrdidio.mongodb.net/?retryWrites=true&w=majority");
            Database = Client.GetDatabase("EnglishWordReminder");
            Collection = Database.GetCollection<BsonDocument>("Words");
        }

        public async Task<Word> FindWord(string word)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Value", word);

            var document = await Collection.Find(filter).FirstOrDefaultAsync();

            if (document == null)
            {
                return null;
            }
            var wordObj = BsonSerializer.Deserialize<Word>(document);

            return wordObj;
        }

        public async Task<bool> AddWord(Word word)
        {
            return await Task.FromResult(true);
        }
    }

    public class Word
    {
        public ObjectId Id { get; set; }
        public string Meaning { get; set; }
        public string Value { get; set; }
    }
}
