using System;
using System.Threading;
using System.Collections.Generic;

namespace Survival_Game_Backend_Server
{
    class Program
    {
        static string command = "";

        private delegate void Command(string[] args);
        static Dictionary<string, Command> commands;

        static bool inputIsInCommand = false;
        
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            Server.Start();

            StartProgramData();

            Console.WriteLine("Server initialized");
            Console.WriteLine("\r\n");

            GetNextLetter();
        }

        static void GetNextLetter()
        {
            ConsoleKeyInfo readKey;

            if (!inputIsInCommand)
            {
                readKey = Console.ReadKey();
                
                if (readKey.Key == ConsoleKey.Enter)
                {
                    Command commandToExecute;

                    Console.WriteLine("\r\n");

                    if (commands.TryGetValue(command, out commandToExecute))
                    {
                        commandToExecute(command.Split(' '));
                    }
                    else
                    {
                        Console.WriteLine("No command with that name was found.");
                        Console.WriteLine("\r\n");
                    }

                    command = "";
                }
                else
                {
                    command += readKey.KeyChar;
                }
            }

            GetNextLetter();
        }

        static void StartProgramData()
        {
            commands = new Dictionary<string, Command>()
            {
                { "get-lobbies", GetCurrentLobbies },
                { "lobby-info", GetLobbyInfo }
            };
        }

        static void GetCurrentLobbies(string[] args)
        {
            Console.WriteLine("\r\n");
            Console.WriteLine($"Lobbies: {Server.lobbies.Count}");
            
            if (Server.lobbies.Count > 0)
            {
                Console.WriteLine("\r\n");
                foreach (Lobby lobby in Server.lobbies)
                {
                    Console.WriteLine($"Lobby {lobby.id}");
                }
            }

            Console.WriteLine("\r\n");
        }

        static void GetLobbyInfo(string[] args)
        {
            inputIsInCommand = true;

            if (args.Length == 1)
            {
                LogError("No lobby ID was specified.");
            }
            else
            {
                string id = args[1];

                if (Server.lobbies.FindIndex(l => l.id == id) > -1)
                {
                    Lobby lobby = Server.lobbies.Find(l => l.id == id);

                    Console.WriteLine($"Lobby {id}");
                    Console.WriteLine("\r\n");
                    Console.WriteLine($"IP: {lobby.ip}");
                    Console.WriteLine("\r\n");
                    Console.WriteLine($"Port: {lobby.port}");
                }
                else
                {
                    LogError("No lobby was found with that id.");
                }
            }

            inputIsInCommand = false;
        }

        static void LogError(string message)
        {
            ConsoleColor pastColor = Console.ForegroundColor;
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.WriteLine("\r\n");
            Console.ForegroundColor = pastColor;
        }
    }
}
 