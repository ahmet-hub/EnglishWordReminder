using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CreateService
{
    public class TranslateService
    {
        string url = "https://translation-api.translate.com/translate/v1/";
        string apiKey = "164354e794d0c3";
        string sourceLanguage = "en";
        string translationLanguage = "tr";
        private HttpClient client;
        public TranslateService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);
            client.DefaultRequestHeaders.Add("accept", "*/*");
            client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", "");
        }
        public async Task<string> TranslateAsync(string input)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("source_language", sourceLanguage),
                    new KeyValuePair<string, string>("translation_language", translationLanguage),
                    new KeyValuePair<string, string>("text", input)
            });

            var response = await client.PostAsync("mt", content);

            if (response.IsSuccessStatusCode)
            {
                var translationResponse = System.Text.Json.JsonSerializer.Deserialize<TranslateResponse>(await response.Content.ReadAsStringAsync());
                var word = translationResponse.Translation.ToLower();
                if (word == input)
                    return null;
                return word;
            }
            else
                return null;
        }

        public Task<bool> CheckServiceAsync()
        {
            return Task.FromResult(true);
        }
    }

    public class TranslateResponse
    {
        [JsonPropertyName("translation")]
        public string Translation { get; set; }
    }
}
