using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsExamplePv421.Models;

public class ProgressBar
{
    private int Width { get; set; } = 20;

    public int Value { get; set; } = 0;
    public int startX { get; set; } = 30;
    public int startY { get; set; } = 7;


    public void Clear()
    {
        lock (Console.Out)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(startX, startY);
            Console.Write("[" + new string(' ', Width) + "]");
            Console.Write($"{0}%".PadLeft(5));
        }
    }

    // [XXXXXXXXXXXXXXXXXXXX] 100%
    public void Draw()
    {
        lock (Console.Out)
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(startX, startY);
            Console.Write("[");
            for (int i = 0; i < 20; i++)
            {
                Console.Write((float)Value / 5 > i ? "X" : ".");
            }
            Console.Write("]");
            Console.Write($"{Value}%".PadLeft(5));
        }
    }
}
