using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LunchAgent.Entities;

namespace LunchAgent.Helpers
{
    public class MenuParser
    {
        public static List<Tuple<RestaurantSettings, List<MenuItem>>> GetMenuFromMenicka(List<RestaurantSettings> restaurantSettingses)
        {
            var result = new List<Tuple<RestaurantSettings, List<MenuItem>>>();

            var document = new HtmlDocument();

            foreach (var setting in restaurantSettingses)
            {
                using (var client = new WebClient())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    var data = setting.Url.Contains("makalu")
                        ? Encoding.UTF8.GetString(client.DownloadData(setting.Url))
                        : Encoding.GetEncoding(1250).GetString(client.DownloadData(setting.Url));

                    document.LoadHtml(data);
                }

                var parsedMenu = setting.Url.Contains("makalu")
                    ? ParseMenuFromMakalu(document.DocumentNode)
                    : ParseMenuFromMenicka(document.DocumentNode);

                result.Add(Tuple.Create(setting, parsedMenu));
            }

            return result;
        }

        private static List<MenuItem> ParseMenuFromMenicka(HtmlNode todayMenu)
        {
            var result = new List<MenuItem>();

            var foodMenus = todayMenu.SelectNodes(".//tr")
                .Where(node => node.GetClasses().Contains("soup") || node.GetClasses().Contains("main"));

            foreach (var food in foodMenus)
            {
                var item = new MenuItem();

                if (food.GetClasses().Contains("soup") == true)
                {
                    item.FoodType = FoodType.Soup;
                    item.Description = Regex.Replace(food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("food")).InnerText, "\\d+.?", string.Empty);
                    item.Price = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("prize")).InnerText;
                }
                else
                {
                    item.FoodType = FoodType.Main;
                    item.Description = Regex.Replace(food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("food")).InnerText, "\\d+.?", string.Empty);
                    item.Price = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("prize")).InnerText;
                    item.Index = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("no")).InnerText;
                }
                result.Add(item);
            }

            return result;
        }

        private static List<MenuItem> ParseMenuFromMakalu(HtmlNode todayMenu)
        {
            var result = new List<MenuItem>();

            var todayString = GetTodayInCzech();

            var todayNode = string.Join(" ", todayMenu.SelectNodes(".//div[contains(@class,TJStrana)]").Where(x => x.GetClasses().Contains("TJStrana")).Select(x=> x.InnerHtml));

            var start = todayNode.IndexOf(todayString) + 13;

            var end = todayNode.Substring(start, todayNode.Length-start).IndexOf("Mix denn");

            var body = todayNode.Substring(start, end);

            var soup = new MenuItem();

            soup.FoodType = FoodType.Soup;
            soup.Description = string.Join(" / ",
                Regex.Matches(body, "[A-ř]+ (polévka|polevka)").Select(x => x.Value));

            var matches = Regex.Matches(body, "<b>.+?<\\/b>").Select(x => x.Value).ToList();

            result.Add(soup);

            foreach (var match in matches)
            {
                var item = new MenuItem();

                item.FoodType = FoodType.Main;
                item.Price = Regex.Match(match, "(?='>)(.*)(?=</span)").Value.Substring(2);
                item.Description = Regex.Match(match, "(?=<b>)(.+?)(?=<span)").Value.Substring(3) + "  ";

                result.Add(item);
            }

            return result;
        }

        private static string GetTodayInCzech()
        {
            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return "Pátek";
                case DayOfWeek.Monday:
                    return "Pondělí";
                case DayOfWeek.Thursday:
                    return "Čtvrtek";
                case DayOfWeek.Tuesday:
                    return "Pátek";
                case DayOfWeek.Wednesday:
                    return "Středa";
            }

            return string.Empty;
        }
    }
}
