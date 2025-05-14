//Невидимий курсор
using System.Diagnostics;
using ThreadsExamplePv421.Models;

Console.CursorVisible = false;

var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

var leftPanel = new FilePanel(currentDirectory, 0);
var rightPanel = new FilePanel(currentDirectory, 60);

leftPanel.OtherPanel = rightPanel;
rightPanel.OtherPanel = leftPanel;


FilePanel activePanel = leftPanel;

while (true)
{
    leftPanel.Draw(leftPanel == activePanel);
    rightPanel.Draw(rightPanel == activePanel);  

    ConsoleKeyInfo key = Console.ReadKey(true);

    switch (key.Key)
    {
        case ConsoleKey.Tab:
            activePanel = activePanel == leftPanel ? rightPanel : leftPanel;
            break;
        case ConsoleKey.Escape:
            Console.WriteLine("Exit");
            return;
        default:
            activePanel.Handle(key.Key);
            break;
    }
}



/*
 * Написати програму консольну, яка виводить на екран вміст папки, в якій вона запущена
 * У вигляді
 * ..
 * dir1          DIR      ---
 * dir2          DIR      ---
 * file1         txt      5,5kb 
 * file2         txt      5.5mb
 * 
 * клавіші вверх вниз - переміщення по списку
 * клавіша Enter - вибір
 * якщо директория - переходити в неї
 * якщо .. переходити на один рівень вверх
 * DEL - видалити файл
 * 
 * 
 * 
 */ 