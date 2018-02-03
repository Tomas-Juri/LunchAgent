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
            var result = string.Empty;

            var data = GetDataFromFile(slackFilePath);

            data["text"] = FormatMenuForSlack(parsedMenus);

            using (var client = new WebClient())
            {
                var response = client.UploadValues(Uri,"POST", data);

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

        private static NameValueCollection GetDataFromFile(string filePath)
        {
            var result = new NameValueCollection();
            var lines = File.ReadAllLines(filePath);

            try
            {
                result["token"] = lines[0];
                result["channel"] = lines[1];
            }
            catch (Exception)
            {
                Console.WriteLine("Error while loading slack file. Please enter valid token on first line and valid channel id to second line");
                throw;
            }

            return result;
        }
    }
}
