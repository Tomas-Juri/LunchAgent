using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using LunchAgent.Entities;
using Newtonsoft.Json.Linq;

namespace LunchLib.Helpers
{
    public class SlackHelper
    {
        private static readonly string PostMessageUri = "https://slack.com/api/chat.postMessage";
        private static readonly string UpdateMessageUri = "https://slack.com/api/chat.update";
        private static readonly string ChatHistoryUri = "https://slack.com/api/channels.history";

        private SlackSetting _slackConfiguration;

        public SlackHelper(SlackSetting configuration)
        {
            _slackConfiguration = configuration;
        }

        public string PostMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            string result;

            var data = new NameValueCollection
            {
                ["token"] = _slackConfiguration.BotToken,
                ["channel"] = _slackConfiguration.ChannelName,
                ["bot_id"] = _slackConfiguration.BotId,
                ["text"] = FormatMenuForSlack(menus)
            };

            using (var client = new WebClient())
            {
                var response = client.UploadValues(PostMessageUri, "POST", data);

                result = new UTF8Encoding().GetString(response);
            }

            return result;
        }


        public string UpdateMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            var timestamp = GetLastMessageTimestamp();

            if (string.IsNullOrEmpty(timestamp))
                return string.Empty;

            var data = new NameValueCollection
            {
                ["token"] = _slackConfiguration.BotToken,
                ["channel"] = _slackConfiguration.ChannelName,
                ["bot_id"] = _slackConfiguration.BotId,
                ["text"] = FormatMenuForSlack(menus)
            };

            data.Remove("bot_id");

            data["text"] = FormatMenuForSlack(menus);
            data["ts"] = timestamp;

            using (var client = new WebClient())
            {
                var response = client.UploadValues(UpdateMessageUri, "POST", data);

                return new UTF8Encoding().GetString(response);
            }
        }

        private string GetLastMessageTimestamp()
        {
            var data = new NameValueCollection
            {
                ["token"] = _slackConfiguration.BotToken,
                ["channel"] = _slackConfiguration.ChannelName,
                ["bot_id"] = _slackConfiguration.BotId,
            };

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

                    var tsInt = (int)Convert.ToDouble(rawTs.Replace(".", ","));

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

        private static string FormatMenuForSlack(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus)
        {
            var result = new List<string>();

            foreach (var parsedMenu in parsedMenus)
            {
                parsedMenu.Item2.FindAll(x => x.FoodType == FoodType.Soup).ForEach(x => x.Description = "_" + x.Description + "_");

                var formatedFood = string.Join(Environment.NewLine, parsedMenu.Item2);

                result.Add($"{parsedMenu.Item1.Emoji}*     {parsedMenu.Item1.Name}*{Environment.NewLine}{formatedFood}");
            }

            return string.Join(Environment.NewLine + Environment.NewLine, result);
        }
    }
}
