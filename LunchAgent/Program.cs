using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LunchAgent.Entities;
using LunchAgent.Helpers;

namespace LunchAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var files = Directory.GetFiles(path + "/JsonData");

            var a = JsonParser.ParseFile(files);

            var parser = new MenuParser();

            var task = parser.GetMenuFromMenicka(a.First().Value.First().Url);

            task.Wait();

            var result = task.Result;
        }
    }
}
