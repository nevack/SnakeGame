using System;
using System.IO;

namespace SnakeGame
{
    internal class Program
    {
        public const string gameName = "Snake";
        public const string gameVersion = "1.3.3";

        private static void Main(string[] args)
        {
            Program.InitializeConsoleParameters();
            GameCore gameCore1;
            try
            {
                int[] numArray = Array.ConvertAll<string, int>(File.ReadAllLines(".\\config.txt"), new Converter<string, int>(int.Parse));
                switch (numArray.Length)
                {
                    case 1:
                        gameCore1 = new GameCore(numArray[0]);
                        break;
                    case 2:
                        GameCore gameCore2 = new GameCore(numArray[0], numArray[1]);
                        break;
                    case 3:
                        GameCore gameCore3 = new GameCore(numArray[0], numArray[1], numArray[2]);
                        break;
                }
            }
            catch (IOException ex)
            {
                gameCore1 = new GameCore();
            }
        }

        private static void SetTitle()
        {
            Console.Title = "Snake 1.3.3";
        }

        private static void InitializeConsoleParameters()
        {
            Program.SetTitle();
            Console.CursorVisible = false;
        }
    }
}
