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
        private readonly HttpClient _webClient;
        private readonly Logger _logger;

        public MenuParser()
        {
            _webClient = new HttpClient();
            _logger = new Logger();
        }

        public async Task<List<MenuItem>> GetMenuFromMenicka(string url)
        {
            var result = new List<MenuItem>();

            var document = new HtmlDocument();

            var response = await _webClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            document.LoadHtml(content);

            result = ParseMenuFromMenicka(document.DocumentNode);

            return result;
        }

        private List<MenuItem> ParseMenuFromMenicka(HtmlNode todayMenu)
        {
            var result = new List<MenuItem>();

            var foodMenus = todayMenu.SelectNodes(".//tr")
                .Where(node => node.GetClasses().Contains("soup") || node.GetClasses().Contains("main"));



            return result;
        }
    }
}
