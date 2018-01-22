using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using LunchAgent.Entities;

namespace LunchAgent.Helpers
{
    public class LoaderHelper
    {
        private readonly WebClient _webClient;
        private readonly Logger _logger;

        public LoaderHelper()
        {
            _webClient = new WebClient();
            _logger = new Logger();
        }

        public List<MenuItem> GetMenuFromMenicka(string url)
        {
            var result = new List<MenuItem>();

            var document = new HtmlDocument();

            document.Load(_webClient.OpenRead(url));

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
