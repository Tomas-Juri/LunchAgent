using System.Collections.Generic;
using System.IO;
using LunchAgent.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LunchAgent.Helpers
{
    public static class JsonParser
    {

        public static List<RestaurantSettings> ParseRestaurantSetting(string file)
        {
            using (var reader = new JsonTextReader(File.OpenText(file)))
            {
                var jsonString = JToken.ReadFrom(reader).ToString();

                var rootObject = JObject.Parse(jsonString)["restaurants"];

                return rootObject.ToObject<List<RestaurantSettings>>();
            }
        }

        public static SlackSetting ParseSlackSetting(string file)
        {
            using (var reader = new JsonTextReader(File.OpenText(file)))
            {
                var jsonString = JToken.ReadFrom(reader).ToString();

                var rootObject = JObject.Parse(jsonString);

                return rootObject.ToObject<SlackSetting>();
            }
        }

    }
}
