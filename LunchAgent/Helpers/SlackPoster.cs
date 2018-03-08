using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using LunchAgent.Entities;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LunchAgent.Helpers
{
    class SlackPoster
    {
        private static readonly string PostMessageUri = "https://slack.com/api/chat.postMessage";
        private static readonly string UpdateMessageUri = "https://slack.com/api/chat.update";
        private static readonly string ChatHistoryUri = "https://slack.com/api/channels.history";

        public static string PostMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus, string slackFilePath)
        {
            var result = string.Empty;

            var data = GetDataFromFile(slackFilePath);

            data["text"] = FormatMenuForSlack(parsedMenus);

            using (var client = new WebClient())
            {
                var response = client.UploadValues(PostMessageUri,"POST", data);

                result = new UTF8Encoding().GetString(response);
            }

            return result;
        }

        public static string FormatMenuForSlack(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus)
        {
            var result = new List<string>();

            foreach (var parsedMenu in parsedMenus)
            {
                parsedMenu.Item2.FindAll(x => x.FoodType == FoodType.Soup).ForEach( x=> x.Description = "_" + x.Description + "_");

                var formatedFood = string.Join(Environment.NewLine, parsedMenu.Item2);

                result.Add(string.Format("{0}*     {1}*{2}{3}",parsedMenu.Item1.Emoji, parsedMenu.Item1.Name, Environment.NewLine, formatedFood));
            }

            return string.Join(Environment.NewLine + Environment.NewLine, result);
        }

        internal static void UpdateMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus, string slackFilePath)
        {
            var timestamp = GetLastMessageTimestamp(slackFilePath);

            if(string.IsNullOrEmpty(timestamp))
                return;

            var data = GetDataFromFile(slackFilePath);

            data.Remove("bot_id");

            data["text"] = FormatMenuForSlack(menus);
            data["ts"] = timestamp;

            using (var client = new WebClient())
            {
                var response = client.UploadValues(UpdateMessageUri, "POST", data);

                var result = new UTF8Encoding().GetString(response);
            }
        }

        private static string GetLastMessageTimestamp(string slackFilePath)
        {
            var data = GetDataFromFile(slackFilePath);

            var timeStamp = DateTime.Today;
            var result = "";

            using (var client = new WebClient())
            {
                var response = client.UploadValues(ChatHistoryUri, "POST", data);

                var stringResponse = new UTF8Encoding().GetString(response);

                dynamic jsonJObject = JObject.Parse(stringResponse);

                var ar = ((JArray)jsonJObject.messages).ToList();

                foreach (dynamic arElement in ar)
                {
                    if (arElement.bot_id != data["bot_id"])
                        continue;

                    string rawTs = arElement.ts;

                    var tsInt = (int) Convert.ToDouble(rawTs.ToString().Replace(".", ","));

                    var tsDate = (new DateTime(1970, 1, 1)).AddSeconds(tsInt);

                    if (tsDate > timeStamp)
                    {
                        timeStamp = tsDate;
                        result = rawTs;
                    }
                }
            }

            return result;
        }

        private static NameValueCollection GetDataFromFile(string filePath)
        {
            var result = new NameValueCollection();
            var lines = File.ReadAllLines(filePath);

            try
            {
                result["token"] = lines[0];
                result["channel"] = lines[1];
                result["bot_id"] = lines[2];
            }
            catch (Exception)
            {
                Console.WriteLine("Error while loading slack file. Please enter valid token on first line and valid channel id to second line");
                throw;
            }

            return result;
        }

        public class SlackMessage
        {
            [JsonProperty]
            public string Text { get; set; }
            [JsonProperty]
            public string Type { get; set; }
            [JsonProperty("ts")]
            public string TimeStamp { get; set; }
        }
    }
}
