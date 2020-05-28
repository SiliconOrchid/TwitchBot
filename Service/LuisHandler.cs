
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TwitchBot.Models;

namespace TwitchBot.Service
{
    //https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-get-started-get-intent-from-rest?pivots=programming-language-csharp

    public class LuisHandler : ILuisHandler
    {
        private LuisConfiguration _luisConfiguration;

        public LuisHandler(IOptions<LuisConfiguration> luisConfiguration)
        {
          _luisConfiguration = luisConfiguration.Value ?? throw new ArgumentNullException(nameof(luisConfiguration));

        }

        public async Task<IntentResponse> GetIntent(string utterance)
        {
            //Todo null checking and other robustness improvements
            return ParseResponse(await MakeRequest(utterance));
        }

        private async Task<string> MakeRequest(string utterance)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _luisConfiguration.AppKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["query"] = utterance;

            // These optional request parameters are set to their default values
            queryString["verbose"] = "true";
            queryString["show-all-intents"] = "true";
            queryString["staging"] = "false";
            queryString["timezoneOffset"] = "0";

            var endpointUri = $"{_luisConfiguration.EndPoint}luis/prediction/v3.0/apps/{_luisConfiguration.AppId}/slots/production/predict?{queryString}";

            var httpResponse = await client.GetAsync(endpointUri);

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

            return new IntentResponse{Intent=topIntent,Certainty=topIntentScore};

        }

    }
    
}