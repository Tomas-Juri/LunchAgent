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

namespace LunchAgent.Helpers
{
    class SlackPoster
    {
        private static readonly string Uri = "https://slack.com/api/chat.postMessage";

        public static string PostMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus, string slackFilePath)
        {
            string result = null;

            var payload = new Payload(slackFilePath);
            //payload.Text = FormatMenuForSlack(parsedMenus);
            payload.Text = "hello there";

            using (var client = new WebClient())
            {
                var data = new NameValueCollection();
                data["token"] = payload.Token;
                data["channel"] = payload.Channel;
                data["text"] = "it works";

                var response = client.UploadValues(Uri,"POST", data);

                result = new UTF8Encoding().GetString(response);
            }

            return result;
        }

        public static string FormatMenuForSlack(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus)
        {
            return string.Empty;
        }
    }

    public class Payload
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public Payload(string slackFilePath)
        {
            var lines = File.ReadAllLines(slackFilePath);

            try
            {
                this.Token = lines[0];
                this.Channel = lines[1];
            }
            catch (Exception)
            {
                Console.WriteLine("Error while loading slack file. Please enter valid token on first line and valid channel id to second line");
                throw;
            }

        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
