using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LunchAgent.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LunchAgent.Helpers
{
    public static class JsonParser
    {
        public static List<RestaurantSettings> ParseFile(string file)
        {
            var result = new List<RestaurantSettings>();

            using (var reader = new JsonTextReader(File.OpenText(file)))
            {
                var jsonString = JToken.ReadFrom(reader).ToString();

                var rootObject = JObject.Parse(jsonString)["restaurants"];

                result = rootObject.ToObject<List<RestaurantSettings>>();
            }

            return result;
        }
    }
}
