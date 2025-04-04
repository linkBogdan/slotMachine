using System;
using System.Text;

class SlotMachine
{
    static int credits = 0;
    static int betIndex = 0;
    static bool keepGambling = true;

    static int[] betOptions = { 5, 10, 15, 20, 25, 50, 100, 150, 200, 250, 500, 1000, 1500, 2000, 2500, 5000, 7500, 10000 };

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Random rng = new Random();

        string[] commonSymbols = { "🍒", "🍋", "🍊", "🍏" };
        string[] uncommonSymbols = { "🍇", "🍉" };
        string[] rareSymbols = { "7 " };

        bool continuePlaying = true;
        while (continuePlaying)
        {
            if (credits <= 0)
            {
                DepositCredits();
            }

            Console.WriteLine("🎰 Welcome to Ultra Hot Slot Machine! 🎰");
            Console.WriteLine("--------------------------------------");

            while (credits > 0)
            {
                DisplayCreditsAndBet();
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    int bet = betOptions[betIndex];
                    if (bet > credits)
                    {
                        DisplayErrorMessage("Insufficient credits for the selected bet. Please select a smaller bet.");
                    }
                    else
                    {
                        Spin(rng, commonSymbols, uncommonSymbols, rareSymbols);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    CycleBetAmount();
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    continuePlaying = false;
                    break;
                }
                else
                {
                    Console.WriteLine("\nPress 'Spacebar' to spin, 'Enter' to change the bet, or 'Escape' to exit.");
                }
            }

            if (credits <= 0 && continuePlaying)
            {
                Console.WriteLine("You've run out of credits!");
                Console.WriteLine("Press 'C' to continue with a new deposit or any other key to exit.");

                ConsoleKeyInfo choice = Console.ReadKey(intercept: true);
                if (choice.Key != ConsoleKey.C)
                {
                    continuePlaying = false;
                }
                Console.Clear();
            }
        }

        Console.WriteLine("Game Over. Thanks for playing!");
    }

    static void DepositCredits()
    {
        int deposit = 0;
        bool validDeposit = false;

        while (!validDeposit)
        {
            Console.Clear();
            Console.WriteLine("Enter your deposit amount (1000 - 500000 credits):");

            string input = Console.ReadLine();

            if (int.TryParse(input, out deposit) && deposit >= 1000 && deposit <= 500000)
            {
                validDeposit = true;
                credits = deposit;
            }
            else
            {
                DisplayErrorMessage("Invalid deposit amount. Please enter a number between 1000 and 500000.");
            }
        }

        Console.Clear();
    }

    static void Spin(Random rng, string[] symbols1, string[] symbols2, string[] symbols3)
    {
        Console.SetCursorPosition(1, 1);
        int bet = betOptions[betIndex]; credits -= bet;
        string[,] grid = new string[3, 3];
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                grid[row, col] = PickSymbolBasedOnRarity(rng, symbols1, symbols2, symbols3);
            }
        }

        ClearConsoleBelow(2); Console.SetCursorPosition(0, 6);
        DisplayGrid(grid);
        if (IsWinningGrid(grid))
        {
            Console.Write($"Credits: {credits}  "); int prize = CalculatePrize(grid, bet, symbols1, symbols2, symbols3); Console.WriteLine($"🎉 You won {prize} credits! 🎉");

            Console.WriteLine("Press 'G' to gamble or any other key to collect the prize.");
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.G)
            {
                GambleForPrize(ref prize);
            }
            else
            {
                credits += prize;
                Console.WriteLine($"Collected {prize} credits.");
            }
        }
        else
        {
            Console.WriteLine("😞 No win.");
        }
    }

    static void DisplayGrid(string[,] grid)
    {
        Console.WriteLine();
        for (int row = 0; row < 3; row++)
        {
            Console.WriteLine($"{grid[row, 0]} | {grid[row, 1]} | {grid[row, 2]}");
        }
        Console.WriteLine();
    }

    static bool IsWinningGrid(string[,] grid)
    {
        return CheckLine(grid[0, 0], grid[0, 1], grid[0, 2]) || CheckLine(grid[1, 0], grid[1, 1], grid[1, 2]) || CheckLine(grid[2, 0], grid[2, 1], grid[2, 2]) || CheckLine(grid[0, 0], grid[1, 1], grid[2, 2]) || CheckLine(grid[0, 2], grid[1, 1], grid[2, 0]);
    }

    static bool CheckLine(string a, string b, string c)
    {
        return a == b && b == c;
    }

    static int CalculatePrize(string[,] grid, int bet, string[] symbols1, string[] symbols2, string[] symbols3)
    {
        int prize = 0;

        string[][] winningLines = new string[][]
{
            new string[] { grid[0, 0], grid[0, 1], grid[0, 2] },              
            new string[] { grid[1, 0], grid[1, 1], grid[1, 2] },              
            new string[] { grid[2, 0], grid[2, 1], grid[2, 2] },              
            new string[] { grid[0, 0], grid[1, 1], grid[2, 2] },              
            new string[] { grid[0, 2], grid[1, 1], grid[2, 0] }           
        };

        foreach (var line in winningLines)
        {
            if (CheckLine(line[0], line[1], line[2]))
            {
                if (Array.Exists(symbols1, symbol => symbol == line[0]))
                {
                    prize += bet * 4;
                }
                else if (Array.Exists(symbols2, symbol => symbol == line[0]))
                {
                    prize += bet * 10;
                }
                else if (Array.Exists(symbols3, symbol => symbol == line[0]))
                {
                    prize += bet * 100;
                }
            }
        }

        return prize;
    }

    static string PickSymbolBasedOnRarity(Random rng, string[] symbols1, string[] symbols2, string[] symbols3)
    {
        int randomValue = rng.Next(10);
        if (randomValue < 7)
        {
            return symbols1[rng.Next(symbols1.Length)];
        }
        else if (randomValue < 9)
        {
            return symbols2[rng.Next(symbols2.Length)];
        }
        else
        {
            return symbols3[rng.Next(symbols3.Length)];
        }
    }

    static void CycleBetAmount()
    {
        int[] availableBets = Array.FindAll(betOptions, bet => bet <= credits);
        if (availableBets.Length > 0)
        {
            betIndex = (betIndex + 1) % availableBets.Length;
        }

        DisplayCreditsAndBet();
    }

    static void DisplayCreditsAndBet()
    {
        ClearSpecificLine(3);
        Console.SetCursorPosition(0, 3);
        Console.Write($"Credits: {credits}  ");
        int currentBet = betOptions[betIndex]; if (currentBet <= credits)
        {
            Console.Write($"Bet: {currentBet}");
        }
        else
        {
            Console.Write("Bet: Not enough credits!");
        }

        Console.WriteLine("\nPress 'Spacebar' to spin, 'Enter' to change the bet, or 'Escape' to exit.");
    }

    static void ClearSpecificLine(int lineNumber)
    {
        Console.SetCursorPosition(0, lineNumber);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, lineNumber);
    }

    static void ClearConsoleBelow(int startLine)
    {
        for (int i = startLine; i < Console.WindowHeight; i++)
        {
            ClearSpecificLine(i);
        }
    }

    static void DisplayErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void GambleForPrize(ref int prize)
    {
        keepGambling = true;

        while (keepGambling && prize > 0)
        {
            Console.WriteLine($"\nGambling {prize} credits. Choose Black (A) or Red (D), or press 'Enter' to collect:");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        keepGambling = false;
                        break;
                    }

                    string playerChoice = GetPlayerChoice(keyInfo);

                    if (playerChoice == null)
                    {
                        Console.WriteLine("Please choose Black (A) or Red (D).");
                        continue;
                    }

                    string correctChoice = (new Random().Next(2) == 0) ? "♠️" : "♥️";
                    Console.WriteLine($"\nThe correct choice was: {correctChoice}");

                    if (playerChoice == correctChoice)
                    {
                        prize *= 2; Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Your prize is now {prize} credits.");
                        Console.ResetColor();
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ClearConsoleBelow(0);
                        Console.SetCursorPosition(0, 6);
                        Console.WriteLine("You lost the gamble!");
                        Console.ResetColor();
                        keepGambling = false;
                        prize = 0;
                        break;
                    }
                }
            }
        }

        if (prize > 0)
        {
            credits += prize;
            Console.WriteLine($"Collected {prize} credits!");
        }
    }

    static string GetPlayerChoice(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.A)
        {
            return "♠️";
        }
        else if (keyInfo.Key == ConsoleKey.D)
        {
            return "♥️";
        }
        else
        {
            return null;
        }
    }
}