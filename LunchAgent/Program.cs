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
            ParameterContainer parameters;
            try
            {
                parameters = LoadArguments(args);
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e);
                return;
            }

            var path = parameters.JsonFilePath;

            var parsedJson = JsonParser.ParseFile(path);

            foreach (var menuSetting in parsedJson)
            {
                
            }


        }

        public static ParameterContainer LoadArguments(string[] args)
        {
            var result = new ParameterContainer();

            if(args.Length != 2 )
                throw new ApplicationException("Invalid number of arguments. Use --help for information about argument usage");

            var jsonFilePath = args[0];
            var slackFilePath = args[1];

            if(File.Exists(jsonFilePath) == false)
                throw new ApplicationException($"Could not find json source file: {jsonFilePath}");

            if (File.Exists(slackFilePath) == false)
                throw new ApplicationException($"Could not find json source file: {slackFilePath}");

            if(Path.GetExtension(jsonFilePath) != ".json")
                throw new ApplicationException("Invalid extestion of json source file");

            result.JsonFilePath = jsonFilePath;
            result.SlackFilePath = slackFilePath;

            return result;
        }

        public struct ParameterContainer
        {
            public string JsonFilePath { get; set; }
            public string SlackFilePath { get; set; }
        }
    }
}
