using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LunchAgent.Entities;
using LunchAgent.Helpers;
using LunchLib.Helpers;

namespace LunchAgent_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = LoadArguments(args);

            switch (arguments.Option)
            {
                case ProgramOption.Help:
                    PrintHelp();
                    return;
                case ProgramOption.Update:
                case ProgramOption.Post:

                    var restaurantSettingses = JsonParser.ParseRestaurantSetting(arguments.JsonFilePath);
                    var slackSettings = JsonParser.ParseSlackSetting(arguments.SlackFilePath);

                    var menus = MenuParser.GetMenuFromMenicka(restaurantSettingses);

                    if (CheckMenu(menus) == false)
                    {
                        ScheduleUdate();
                    }
                    else
                    {
                        ScheduleEnd();
                    }

                    var slackHelper = new SlackHelper(slackSettings);

                    if (arguments.Option == ProgramOption.Post)
                    {
                        slackHelper.PostMenu(menus);
                    }
                    else
                    {
                        slackHelper.UpdateMenu(menus);
                    }

                    break;
            }
        }


        private static bool CheckMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            return !menus.Any(x => x.Item2.Any(y => y.Description.Contains("Pro tento den nebylo zadáno menu.")));
        }

        private static void ScheduleUdate()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var filename = location + "\\update.txt";

            if (File.Exists(filename) == false)
            {
                File.WriteAllLines(filename, new[] { DateTime.Now.ToString() });
            }
        }

        private static void ScheduleEnd()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var filename = location + "\\update.txt";

            if (File.Exists(filename) == true)
            {
                File.Delete(filename);
            }

            filename = location + "\\done.txt";

            if (File.Exists(filename) == false)
            {
                File.WriteAllLines(filename, new[] { DateTime.Now.ToString() });
            }
        }

        public static void PrintHelp()
        {

        }

        private static ArgumentContainer LoadArguments(string[] args)
        {
            var result = new ArgumentContainer();

            if (args.Contains("--help"))
            {
                result.Option = ProgramOption.Help;
                return result;
            }

            if (args.Length != 3)
                throw new ArgumentException("Invalid number of arguments");


            result.JsonFilePath = args[0];
            result.SlackFilePath = args[1];

            if (Enum.TryParse(args[2], out ProgramOption parseResult) == false)
                throw new ArgumentException("Invalid option");

            result.Option = (ProgramOption)parseResult;

            if (File.Exists(result.JsonFilePath) == false)
                throw new FileNotFoundException("Could not find specified json file");

            if (File.Exists(result.SlackFilePath) == false)
                throw new FileNotFoundException("Could not find specified slack file");

            return result;
        }

        public struct ArgumentContainer
        {
            public string JsonFilePath { get; set; }
            public string SlackFilePath { get; set; }
            public ProgramOption Option { get; set; }
        }

        public enum ProgramOption
        {
            Help,
            Update,
            Post
        }
    }
}
