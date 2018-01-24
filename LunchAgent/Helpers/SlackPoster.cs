using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using LunchAgent.Entities;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace LunchAgent.Helpers
{
    class SlackPoster
    {
        public static string PostMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus, string slackFilePath)
        {
            var slackKey = System.IO.File.ReadAllLines(slackFilePath).First();

            if (string.IsNullOrEmpty(slackKey))
            {
                throw new ApplicationException($"Cannot post to slack because file '{slackFilePath}' contains no slack key on first line");
            }

            var payload = new Payload();

            payload.Text = FormatMenuForSlack(parsedMenus);

            using (var client = new WebClient())
            {
                var data = new NameValueCollection();

                data["payload"] = payload.ToJson();

                var response = client.UploadValues(slackKey, "POST", data);

                return new UTF8Encoding().GetString(response);
            }
        }

        public static string FormatMenuForSlack(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus)
        {
            return string.Empty;
        }
    }

    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
