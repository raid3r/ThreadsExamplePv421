using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsExamplePv421.Models;

public class TaskInfoPanel
{
    public int Width { get; set; } = 40;
    public int Height { get; set; } = 10;
    private int startX = 30;
    private int startY = 5;

    public string Title { get; set; } = string.Empty;

    

    public void Clear()
    {
        DrawFrame(ConsoleColor.Black);
    }

    private void DrawFrame(ConsoleColor background)
    {
        lock (Console.Out)
        {

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Console.SetCursorPosition(startX + j, startY + i);
                    Console.BackgroundColor = background;
                    Console.Write(" ");
                }
            }
            
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(startX, startY);
            Console.Write(Title);

            Console.ResetColor();
        }
        
    }

    public void Draw()
    {
        DrawFrame(ConsoleColor.DarkGray);
    }

}
