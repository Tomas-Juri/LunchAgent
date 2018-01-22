using System;
using System.Collections.Generic;
using System.IO;
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
        }
    }
}
