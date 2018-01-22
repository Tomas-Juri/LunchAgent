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
        public static Dictionary<string,List<RestaurantSettings>> ParseFile(params string[] files)
        {
            var result = new Dictionary<string, List<RestaurantSettings>>();

            foreach (var file in files)
            {
                using (var reader = new JsonTextReader(File.OpenText(file)))
                {
                    var jsonString = JToken.ReadFrom(reader).ToString();

                    var rootObject = JObject.Parse(jsonString)["restaurants"];

                    var filename = Path.GetFileNameWithoutExtension(file);

                    result[filename] = rootObject.ToObject<List<RestaurantSettings>>();
                }
            }

            return result;
        }
    }
}
