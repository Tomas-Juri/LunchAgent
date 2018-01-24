using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LunchAgent.Entities;

namespace LunchAgent.Helpers
{
    public class MenuParser
    {
        public static async Task<List<Tuple<RestaurantSettings, List<MenuItem>>>> GetMenuFromMenicka(List<RestaurantSettings> restaurantSettingses)
        {
            var result = new List<Tuple<RestaurantSettings, List<MenuItem>>>();

            var webClient = new HttpClient();

            var document = new HtmlDocument();

            foreach (var setting in restaurantSettingses)
            {
                var response = await webClient.GetAsync(setting.Url);

                var content = await response.Content.ReadAsStringAsync();

                document.LoadHtml(content);

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



            return result;
        }
    }
}
