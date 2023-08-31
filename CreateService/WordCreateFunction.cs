using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace CreateService
{
    public static class WordCreateFunction
    {
        [FunctionName("Create")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [ServiceBus(queueOrTopicName: "", Connection = "")] IAsyncCollector<Word> serviceBusCollector)
        {
            if (req.Query.TryGetValue("word", out var word))
            {
                word = word.ToString().ToLower();
                var mongoService = new MongoService();
                var entity = await mongoService.FindWord(word);

                if (entity == null)
                {
                    var translateService = new TranslateService();

                    var isAvailable = await translateService.CheckServiceAsync();

                    if (!isAvailable)
                        return new OkObjectResult("Translation limit exceeded");

                    var meaning = await translateService.TranslateAsync(word);

                    if (meaning == null)
                        return new OkObjectResult("Incorrect request");

                    entity = new Word
                    {
                        Value = word,
                        Meaning = meaning
                    };

                    await mongoService.AddWord(entity);
                }
                await serviceBusCollector.AddAsync(entity);

                return new OkObjectResult("Your request has been received.");
            }
            return new OkObjectResult("Incorrect request");
        }
    }
}
