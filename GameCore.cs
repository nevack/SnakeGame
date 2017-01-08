using System;
using System.Threading;

namespace SnakeGame
{
    public class GameCore
    {
        private const int standartWidth = 100;
        private const int standartHeight = 40;
        private const int standartSpeed = 150;
        public const ConsoleColor BorderColor = ConsoleColor.Yellow;
        public const ConsoleColor SnakeColor = ConsoleColor.Green;
        public const ConsoleColor AppleColor = ConsoleColor.Red;
        private int currentSnakeHeadColumn;
        private int currentSnakeHeadRow;
        private int currentSnakeLength;
        private readonly int fieldHeight;
        private readonly int fieldWidth;
        private readonly int gameSpeed;
        private EMove moveDirection;
        private EGameState gameState;
        private int[,] gameField;

        public GameCore()
        {
            GameCore gameCore = new GameCore(100, 40, 33);
        }

        public GameCore(int width, int height)
        {
            GameCore gameCore = new GameCore(width, height, 150);
        }

        public GameCore(int speed)
        {
            GameCore gameCore = new GameCore(100, 40, speed);
        }

        public GameCore(int width, int height, int speed)
        {
            if (width < 80 || width > 160)
                width = 100;
            if (height < 25 || height > 55)
                height = 40;
            if (speed < 16 || speed > 500)
                speed = 150;
            Console.SetWindowSize(width + 1, height + 1);
            Console.SetBufferSize(width + 1, height + 1);
            this.fieldWidth = width;
            this.fieldHeight = height;
            this.gameSpeed = speed;
            this.gameField = new int[width, height];
            this.StartGame();
        }

        private void StartGame()
        {
            Console.Clear();
            this.ShowHelloMessage();
            Console.Clear();
            this.gameState = EGameState.Start;
            Array.Clear((Array)this.gameField, 0, this.gameField.Length);
            this.DrawBorders();
            this.SetSnakePosition();
            this.moveDirection = EMove.RIGHT;
            while (this.gameState != EGameState.Exit)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.P:
                        this.GamePause();
                        break;
                    case ConsoleKey.S:
                        if (this.moveDirection != EMove.DOWN)
                        {
                            this.moveDirection = EMove.UP;
                            break;
                        }
                        break;
                    case ConsoleKey.W:
                        if (this.moveDirection != EMove.UP)
                        {
                            this.moveDirection = EMove.DOWN;
                            break;
                        }
                        break;
                    case ConsoleKey.Escape:
                        this.gameState = EGameState.Exit;
                        break;
                    case ConsoleKey.A:
                        if (this.moveDirection != EMove.RIGHT)
                        {
                            this.moveDirection = EMove.LEFT;
                            break;
                        }
                        break;
                    case ConsoleKey.D:
                        if (this.moveDirection != EMove.LEFT)
                        {
                            this.moveDirection = EMove.RIGHT;
                            break;
                        }
                        break;
                }
                while (this.gameState != EGameState.Exit && !Console.KeyAvailable)
                {
                    Thread.Sleep(this.gameSpeed);
                    if (!this.CheckMoveAvailable())
                    {
                        this.Move();
                        this.ShowScore();
                    }
                    else
                    {
                        this.gameState = EGameState.Exit;
                        this.ShowGameOver();
                    }
                }
            }
        }

        private void DrawBorders()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int left = 0; left < this.fieldWidth; ++left)
            {
                this.gameField[left, 0] = this.gameField[left, 1] = this.gameField[left, this.fieldHeight - 1] = -1;
                Console.SetCursorPosition(left, 1);
                Console.Write("#");
                Console.SetCursorPosition(left, this.fieldHeight - 1);
                Console.Write("#");
            }
            for (int top = 2; top < this.fieldHeight - 1; ++top)
            {
                this.gameField[0, top] = this.gameField[this.fieldWidth - 1, top] = -1;
                Console.SetCursorPosition(0, top);
                Console.Write("#");
                Console.SetCursorPosition(this.fieldWidth - 1, top);
                Console.Write("#");
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(this.fieldWidth - 20, 0);
            Console.Write("Press ESC to exit.");
            this.ReturnCursor();
        }

        private void SetSnakePosition()
        {
            this.currentSnakeHeadColumn = this.fieldWidth / 2 + 1;
            this.currentSnakeHeadRow = this.fieldHeight / 2;
            this.currentSnakeLength = 3;
            this.gameField[this.fieldWidth / 2 - 1, this.fieldHeight / 2] = 3;
            this.gameField[this.fieldWidth / 2, this.fieldHeight / 2] = 2;
            this.gameField[this.fieldWidth / 2 + 1, this.fieldHeight / 2] = 1;
            this.PlaceApple();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(this.fieldWidth / 2 - 1, this.fieldHeight / 2);
            Console.Write("0");
            Console.SetCursorPosition(this.fieldWidth / 2, this.fieldHeight / 2);
            Console.Write("0");
            Console.SetCursorPosition(this.fieldWidth / 2 + 1, this.fieldHeight / 2);
            Console.Write("@");
            this.ReturnCursor();
        }

        private void ReturnCursor()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(0, 0);
        }

        private void ShowGameOver()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition((this.fieldWidth - 62) / 2, this.fieldHeight / 2 - 3);
            Console.WriteLine("  ________                        ________                      ");
            Console.SetCursorPosition((this.fieldWidth - 62) / 2, this.fieldHeight / 2 - 2);
            Console.WriteLine(" /  _____/_____    _____   ____   \\_____  \\___  __ ___________  ");
            Console.SetCursorPosition((this.fieldWidth - 62) / 2, this.fieldHeight / 2 - 1);
            Console.WriteLine("/   \\  ___\\__  \\  /     \\_/ __ \\   /   |   \\  \\/ // __ \\_  __ \\ ");
            Console.SetCursorPosition((this.fieldWidth - 62) / 2, this.fieldHeight / 2);
            Console.WriteLine("\\    \\_\\  \\/ __ \\|  | |  \\  ___/  /    |    \\   /\\  ___/|  | \\/ ");
            Console.SetCursorPosition((this.fieldWidth - 62) / 2, this.fieldHeight / 2 + 1);
            Console.WriteLine(" \\______  (____  /__|_|  /\\___  > \\_______  /\\_/  \\___  >__|    ");
            Console.SetCursorPosition((this.fieldWidth - 62) / 2, this.fieldHeight / 2 + 2);
            Console.WriteLine("        \\/     \\/      \\/     \\/          \\/          \\/        ");
            string str = "Your Score is " + (object)this.currentSnakeLength;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition((this.fieldWidth - str.Length) / 2, this.fieldHeight / 2 + 4);
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((this.fieldWidth - 52) / 2, this.fieldHeight - 3);
            Console.WriteLine("Press R to restart the game or any other key to exit!");

            if (Console.ReadKey(true).Key != ConsoleKey.R)
                return;
            this.StartGame();
        }

        private void GamePause()
        {
            do;
            while (!Console.KeyAvailable);
        }

        private bool CheckMoveAvailable()
        {
            return this.moveDirection == EMove.LEFT && (this.gameField[this.currentSnakeHeadColumn - 1, this.currentSnakeHeadRow] != 0 && this.gameField[this.currentSnakeHeadColumn - 1, this.currentSnakeHeadRow] != -2) || this.moveDirection == EMove.RIGHT && (this.gameField[this.currentSnakeHeadColumn + 1, this.currentSnakeHeadRow] != 0 && this.gameField[this.currentSnakeHeadColumn + 1, this.currentSnakeHeadRow] != -2) || (this.moveDirection == EMove.UP && (this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow + 1] != 0 && this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow + 1] != -2) || this.moveDirection == EMove.DOWN && (this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow - 1] != 0 && this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow - 1] != -2));
        }

        private void PlaceApple()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Random random = new Random();
            int top = random.Next(2, 24);
            int left = random.Next(1, 79);
            if (this.gameField[left, top] == 0)
            {
                this.gameField[left, top] = -2;
                Console.SetCursorPosition(left, top);
                Console.Write("$");
            }
            else
                this.PlaceApple();
            this.ReturnCursor();
        }

        private void Move()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            for (int index1 = 1; index1 < this.fieldWidth - 1; ++index1)
            {
                for (int index2 = 1; index2 < this.fieldHeight - 1; ++index2)
                {
                    if (this.gameField[index1, index2] > 0)
                        ++this.gameField[index1, index2];
                }
            }
            bool flag = this.CheckApple();
            Console.SetCursorPosition(this.currentSnakeHeadColumn, this.currentSnakeHeadRow);
            Console.Write("0");
            if (this.moveDirection == EMove.UP)
            {
                this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow + 1] = 1;
                Console.SetCursorPosition(this.currentSnakeHeadColumn, this.currentSnakeHeadRow + 1);
                Console.Write("@");
                ++this.currentSnakeHeadRow;
            }
            if (this.moveDirection == EMove.DOWN)
            {
                this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow - 1] = 1;
                Console.SetCursorPosition(this.currentSnakeHeadColumn, this.currentSnakeHeadRow - 1);
                Console.Write("@");
                --this.currentSnakeHeadRow;
            }
            if (this.moveDirection == EMove.RIGHT)
            {
                this.gameField[this.currentSnakeHeadColumn + 1, this.currentSnakeHeadRow] = 1;
                Console.SetCursorPosition(this.currentSnakeHeadColumn + 1, this.currentSnakeHeadRow);
                Console.Write("@");
                ++this.currentSnakeHeadColumn;
            }
            if (this.moveDirection == EMove.LEFT)
            {
                this.gameField[this.currentSnakeHeadColumn - 1, this.currentSnakeHeadRow] = 1;
                Console.SetCursorPosition(this.currentSnakeHeadColumn - 1, this.currentSnakeHeadRow);
                Console.Write("@");
                --this.currentSnakeHeadColumn;
            }
            if (!flag)
            {
                for (int left = 1; left < this.fieldWidth - 1; ++left)
                {
                    for (int top = 1; top < this.fieldHeight - 1; ++top)
                    {
                        if (this.gameField[left, top] > this.currentSnakeLength)
                        {
                            this.gameField[left, top] = 0;
                            Console.SetCursorPosition(left, top);
                            Console.Write(" ");
                        }
                    }
                }
            }
            else
            {
                this.PlaceApple();
                ++this.currentSnakeLength;
            }
            this.ReturnCursor();
        }

        private bool CheckApple()
        {
            return this.moveDirection == EMove.LEFT && this.gameField[this.currentSnakeHeadColumn - 1, this.currentSnakeHeadRow] == -2 || this.moveDirection == EMove.RIGHT && this.gameField[this.currentSnakeHeadColumn + 1, this.currentSnakeHeadRow] == -2 || (this.moveDirection == EMove.UP && this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow + 1] == -2 || this.moveDirection == EMove.DOWN && this.gameField[this.currentSnakeHeadColumn, this.currentSnakeHeadRow - 1] == -2);
        }

        private void ShowHelloMessage()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition((this.fieldWidth - 39) / 2, this.fieldHeight / 2 - 7);
            Console.WriteLine("  _________              __           ");
            Console.SetCursorPosition((this.fieldWidth - 39) / 2, this.fieldHeight / 2 - 6);
            Console.WriteLine(" /   _____/ ____ _____  |  | __ ____  ");
            Console.SetCursorPosition((this.fieldWidth - 39) / 2, this.fieldHeight / 2 - 5);
            Console.WriteLine(" \\_____  \\ /    \\\\__  \\ |  |/ // __ \\ ");
            Console.SetCursorPosition((this.fieldWidth - 39) / 2, this.fieldHeight / 2 - 4);
            Console.WriteLine(" /        \\   |  \\/ __ \\|    <\\  ___/ ");
            Console.SetCursorPosition((this.fieldWidth - 39) / 2, this.fieldHeight / 2 - 3);
            Console.WriteLine("/_______  /___|  (____  /__|_ \\\\___  >");
            Console.SetCursorPosition((this.fieldWidth - 39) / 2, this.fieldHeight / 2 - 2);
            Console.WriteLine("        \\/     \\/     \\/     \\/    \\/ ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((this.fieldWidth - 9) / 2, this.fieldHeight / 2 - 1);
            Console.WriteLine("by nevack");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition((this.fieldWidth - 40) / 2, this.fieldHeight / 2 + 3);
            Console.Write("         ┌───┐             ┌───┐");
            Console.SetCursorPosition((this.fieldWidth - 40) / 2, this.fieldHeight / 2 + 4);
            Console.Write("         │ W ├─UP          │ P ├─PAUSE");
            Console.SetCursorPosition((this.fieldWidth - 40) / 2, this.fieldHeight / 2 + 5);
            Console.Write("     ┌───┼───┼───┐         └───┘");
            Console.SetCursorPosition((this.fieldWidth - 40) / 2, this.fieldHeight / 2 + 6);
            Console.Write("LEFT─┤ A │ S │ D ├─RIGHT");
            Console.SetCursorPosition((this.fieldWidth - 40) / 2, this.fieldHeight / 2 + 7);
            Console.Write("     └───┴─┬─┴───┘");
            Console.SetCursorPosition((this.fieldWidth - 40) / 2, this.fieldHeight / 2 + 8);
            Console.Write("           └─DOWN");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((this.fieldWidth - 24) / 2, this.fieldHeight - 2);
            Console.Write("Press any key to start.");
            this.ReturnCursor();
            do
                ;
            while (!Console.KeyAvailable);
        }

        private void ShowScore()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition((this.fieldWidth - 14) / 2, 0);
            Console.Write("Your Score: " + (object)this.currentSnakeLength);
            this.ReturnCursor();
        }
    }
}
