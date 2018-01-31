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

                    var data = Encoding.GetEncoding(1250).GetString(client.DownloadData(setting.Url));

                    document.LoadHtml(data);
                }

                var parsedMenu = ParseMenuFromMenicka(document.DocumentNode);

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
    }
}
