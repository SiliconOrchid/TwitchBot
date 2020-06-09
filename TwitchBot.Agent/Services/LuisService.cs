using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using TwitchBot.Agent.Services.Interfaces;
using TwitchBot.Common.Models.Chat;
using TwitchBot.Common.Models.Configuration;

namespace TwitchBot.Agent.Services
{
    public class LuisService : ILuisService
    {
        private LuisConfiguration _luisConfiguration;

        private HttpClient _httpClient;

        public LuisService(IOptions<LuisConfiguration> luisConfiguration)
        {
            _luisConfiguration = luisConfiguration.Value ?? throw new ArgumentNullException(nameof(luisConfiguration));

            _httpClient = new HttpClient();
        }

        public async Task<IntentResponse> GetIntentAsync(string utterance)
        {
            //Todo null checking and other robustness improvements

            string response = await MakeRequestAsync(utterance);
            return ParseResponse(response);
        }

        private async Task<string> MakeRequestAsync(string utterance)
        {
            //https://eu.luis.ai/
            //

            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _luisConfiguration.AppKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["query"] = utterance;

            // These optional request parameters are set to their default values
            queryString["verbose"] = "true";
            queryString["show-all-intents"] = "true";
            queryString["staging"] = "false";
            queryString["timezoneOffset"] = "0";

            var endpointUri = $"{_luisConfiguration.EndPoint}luis/prediction/v3.0/apps/{_luisConfiguration.AppId}/slots/production/predict?{queryString}";

            var httpResponse = await _httpClient.GetAsync(endpointUri);

            var strResponseContent = await httpResponse.Content.ReadAsStringAsync();

            return strResponseContent;
        }



        //https://dotnetcoretutorials.com/2019/09/11/how-to-parse-json-in-net-core/
        private IntentResponse ParseResponse(string json)
        {
            var parsedJObject = JObject.Parse(json);
            string topIntent = parsedJObject.SelectToken("$.prediction.topIntent").Value<string>();

            //todo use tryparse for robustness
            decimal topIntentScore = decimal.Parse(parsedJObject.SelectToken($"$.prediction.intents.{topIntent}.score").Value<string>());



            //string embeddedUrlIfAvailable = parsedJObject.SelectToken($"$.prediction.entities.url[0]").Value<string>();
            string embeddedUrlIfAvailable = (string)parsedJObject.SelectToken($"$.prediction.entities.url[0]");


            return new IntentResponse { Intent = topIntent, Certainty = topIntentScore, EmbeddedUrl = embeddedUrlIfAvailable };

        }

    }

}
