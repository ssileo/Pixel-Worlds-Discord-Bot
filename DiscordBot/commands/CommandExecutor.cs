using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.commands.list;

namespace TcpServer.commands
{
    class CommandExecutor
    {
        public static readonly string prefix = "";

        public static List<ToggleCommand> toggleCommands = new List<ToggleCommand>();
        public static List<SimpleCommand> simpleCommands = new List<SimpleCommand>();


        private static void MakeSimple(SimpleCommand cmd, List<string> handlers, string description = "")
        {
            cmd.handlers = handlers;
            cmd.description = description;
            simpleCommands.Add(cmd);
        }

        public static void MakeToggle(bool state, List<string> handlers, string description = "")
        {
            ToggleCommand cmd = new ToggleCommand();
            cmd.handlers = handlers;
            cmd.state = state;
            cmd.description = description;
            toggleCommands.Add(cmd);
        }

        public static bool Execute(string cmd)
        {
            string[] args = cmd.Split(' ');

            string cmd_full = cmd;

            cmd = args[0].ToLower();

            if (cmd.StartsWith(prefix))
            {
                var commands = toggleCommands;

                foreach (var command in commands)
                {
                    foreach (var handler in command.handlers)
                    {
                        if (prefix + handler == cmd)
                        {
                            command.state = !command.state;
                                 Console.WriteLine($"{handler}: " + (command.state ? "True" : "False"));
                                 Console.WriteLine("CommandExecutor -> Executed: " + cmd);
                            return true;
                        }
                    }
                }

                var command2 = simpleCommands;
                foreach (var command in command2)
                {
                    foreach (var handler in command.handlers)
                    {
                        if (prefix + handler == cmd)
                        {
                            command.execute(cmd_full, args);
                            Console.WriteLine("CommandExecutor -> Executed: " + cmd);
                            return true;
                        }
                    }
                }

                Console.WriteLine($"Unknown command. Try {prefix}help for a list of commands");
                Console.WriteLine("CommandExecutor -> Unknown command: " + cmd);
                return true;
            }
            return false;
        }



        public static void LoadCommands()
        {
            LoadSimpleCommands();
            // MakeToggle(GameController.debug, new List<string>() { "debug" }, "Get the debug logs");
        }
        private static void LoadSimpleCommands()
        {
            // utils
            MakeSimple(new Join(), new List<string>() { "join", "warp" }, "Joins a world");
            
        }
    }
}
