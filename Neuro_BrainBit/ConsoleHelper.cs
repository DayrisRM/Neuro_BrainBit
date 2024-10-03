using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuro_BrainBit
{
    public static class ConsoleHelper
    {
        public static void WriteLine(int pad, string text, ConsoleColor color = default)
        {
            if (color != default)
                Console.ForegroundColor = color;

            Console.WriteLine(string.Empty.PadLeft(pad, ' ') + text.ToString());

            if (color != default)
                Console.ResetColor();
        }
              

        public static Dictionary<string, Command> InitializeCommands()
        {
            var commands = new Dictionary<string, Command>
            {
                { "help", new Command("help", "Show the available commands.") },
                { "search", new Command("search", "Search devices.") },
                { "connect", new Command("connect", "Connect to the first device.") },
                { "search-and-connect", new Command("search-and-connect", "Search and connecto to the first device.") },
                { "disconnect", new Command("disconnect", "Disconnect.") },
                { "battery-start", new Command("battery-start", "Subscribe to the battery event.") },
                { "battery-stop", new Command("battery-stop", "Unsubscribe to the battery event.") },
                { "resistance", new Command("resistance <seconds>", "Get the resistance for <seconds>.") },
                { "connection-state-start", new Command("connection-state-start", "Subscribe to connection state event.") },
                { "connection-state-end", new Command("connection-state-end", "Unsubscribe to connection state event.") },                
                { "start-signal-print", new Command("start-signal-print <seconds>", "Print the signal values for <seconds>.") },
                { "emotion-start", new Command("emotion-start <seconds>", "Start the emotion library. It is active for <seconds>.") },
                { "emotion-stopt", new Command("emotion-stop", "Stop the emotion library.") }
            };

            return commands;
        }

        public static void ShowHelp(Dictionary<string, Command> commands)
        {
            Console.WriteLine("Commands:");
            foreach (var command in commands.Values)
            {
                Console.WriteLine($"{command.Usage}: {command.Description}");
            }
        }

        public static Tuple<string, string> ExtractParameters(string commandText) 
        {       
            commandText = commandText.ToLower().Trim();

            string[] parts = commandText.Split(' ', 2);
            string command = parts[0];
            string parameter = parts.Length > 1 ? parts[1] : string.Empty;

            return new Tuple<string, string>(command, parameter);
        }

        public static int GetSeconds(string param) 
        {
            var secs = 10;

            if(!string.IsNullOrEmpty(param))
                int.TryParse(param, out secs);

            return secs;
        }

    }
}
