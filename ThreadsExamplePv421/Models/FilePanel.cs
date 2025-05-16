using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsExamplePv421.Models;

public class FilePanel
{
    private DirectoryInfo currentDirectory;
    private int activeIndex = 0;

    private readonly int maxWidth = 59;
    private int startX;

    /// <summary>
    /// Конструктор класу FilePanel
    /// </summary>
    /// <param name="currentDirectory">Директорія для відображення</param>
    /// <param name="startX">Позиція початку панелі</param>
    public FilePanel(DirectoryInfo currentDirectory, int startX)
    {
        this.currentDirectory = currentDirectory;
        this.activeIndex = 0;
        this.startX = startX;
    }


    public FilePanel OtherPanel { get; set; } = null!; // для копіювання файлів між панелями

    /// <summary>
    /// Метод для відображення панелі
    /// </summary>
    /// <param name="isActive"></param>
    public void Draw(bool isActive)
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(startX, 0);

            if (isActive)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
            }

            if (currentDirectory.FullName.Length > maxWidth)
                Console.WriteLine(currentDirectory.FullName.Substring(currentDirectory.FullName.Length - maxWidth));
            else
                Console.WriteLine(currentDirectory.FullName);

            Console.ResetColor();

            var items = GetPanelItems(currentDirectory);
            for (int i = 0; i < items.Count; i++)
            {
                Console.SetCursorPosition(startX, i + 1);
                // якщо елемент активний - виділяємо його
                if (i == activeIndex && isActive)
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(items[i].Display);

                Console.ResetColor();
            }
        }
    }

    /// <summary>
    /// Метод для обробки натиснутої клавіші
    /// </summary>
    /// <param name="key"></param>
    public void Handle(ConsoleKey key)
    {
        var items = GetPanelItems(currentDirectory);

        switch (key)
        {
            case ConsoleKey.PageUp:
                activeIndex = 0;
                break;
            case ConsoleKey.PageDown:
                activeIndex = items.Count - 1;
                break;
            case ConsoleKey.UpArrow:                        //   0  1  [2]
                if (activeIndex == 0)
                {
                    activeIndex = items.Count - 1;
                }

                else
                    activeIndex--;
                break;
            case ConsoleKey.DownArrow:
                if (activeIndex == items.Count - 1)
                {
                    activeIndex = 0;
                }

                else
                    activeIndex++;
                break;
            case ConsoleKey.Enter:
                HandleUseItem();
                break;
            case ConsoleKey.Delete:
                HandleDelete();
                break;
            case ConsoleKey.F5:
                HandleCopyKey();
                break;
            case ConsoleKey.F6:
                HandleMoveKey();
                break;

            case ConsoleKey.E: // encrypt
                HandleEncryptFile();
                break;

            case ConsoleKey.D: // encrypt
                HandleDecryptFile();
                break;

            case ConsoleKey.F1: // test
                var taskPanel = new TaskInfoPanel()
                {
                    Title = "Encode files",
                    Width = 31,
                    Height = 8,
                };
                taskPanel.Draw();

                var work = true;
                var rand = new Random();

                var threads = new List<Thread>();
                for (int i = 0; i < 10; i++)
                {
                    threads.Add(new Thread(() =>
                    {
                        var progressBar = new ProgressBar() { Value = 0, startY = rand.Next(0, 20), startX = rand.Next(0, 50) };
                        for (int i = 0; i < 100; i++)
                        {
                            if (!work)
                            {
                                progressBar.Clear();
                                break;
                            }

                            progressBar.Value = i;
                            progressBar.Draw();
                            Thread.Sleep(rand.Next(100, 1000));
                        }
                    }));
                }
                threads.ForEach(t => t.Start());



                Console.ReadKey(true);
                work = false;

                taskPanel.Clear();

                break;

            default:
                break;
        }
    }


    private List<DirectoryItemViewModel> GetPanelItems(DirectoryInfo directory)
    {
        List<DirectoryItemViewModel> list = new();
        list.Add(new DirectoryItemViewModel { Display = "..", Name = ".." });
        foreach (var d in directory.GetDirectories())
        {

            list.Add(new DirectoryItemViewModel
            {
                ///   "hello                          "

                Display = d.Name.PadRight(50, '.') + ("DIR").PadLeft(maxWidth - 50, '.'),
                Name = d.Name
            });
        }
        foreach (var f in directory.GetFiles())
        {
            var unit = "b";
            long size = f.Length;
            if (f.Length > 1024 * 1024)
            {
                unit = "mb";
                size = f.Length / 1024 / 1024;
            }
            else if (f.Length > 1024)
            {
                unit = "kb";
                size = f.Length / 1024;
            }

            var fileName = f.Name;
            if (fileName.Length > maxWidth)
                fileName = fileName.Substring(fileName.Length - maxWidth);

            list.Add(new DirectoryItemViewModel
            {
                Display = fileName.PadRight(50, '.') + (size.ToString() + ' ' + unit).PadLeft(maxWidth - 50, '.'),
                Name = f.Name
            });
        }
        return list;
    }

    private void HandleUseItem()
    {
        var items = GetPanelItems(currentDirectory);
        var selectedItem = items[activeIndex];
        if (selectedItem.Name == "..")
        {
            // Повертаємось на один рівень вверх
            currentDirectory = currentDirectory.Parent;
            activeIndex = 0;
            Console.Clear();
            return;
        }
        var selectedDir = currentDirectory.GetDirectories().FirstOrDefault(d => d.Name == selectedItem.Name);
        if (selectedDir != null)
        {
            currentDirectory = selectedDir;
            activeIndex = 0;
            Console.Clear();
            return;
        }
        var selectedFile = currentDirectory.GetFiles().FirstOrDefault(f => f.Name == selectedItem.Name);
        if (selectedFile != null)
        {
            // Тут можна додати код для відкриття файлу
            Process.Start(new ProcessStartInfo(selectedFile.FullName) { UseShellExecute = true });
        }
    }
    private void HandleDelete()
    {
        var items = GetPanelItems(currentDirectory);
        var selectedFileForDelete = currentDirectory.GetFiles().FirstOrDefault(f => f.Name == items[activeIndex].Name);
        if (selectedFileForDelete != null)
        {
            selectedFileForDelete.Delete();
            Console.Clear();
        }
    }
    private void HandleCopyKey()
    {
        var items = GetPanelItems(currentDirectory);
        var selectedFileForCopy = currentDirectory.GetFiles().FirstOrDefault(f => f.Name == items[activeIndex].Name);
        if (selectedFileForCopy != null)
        {
            var otherPanelDirectory = OtherPanel.currentDirectory;
            var newFileName = Path.Combine(OtherPanel.currentDirectory.FullName, selectedFileForCopy.Name);
            File.Copy(selectedFileForCopy.FullName, newFileName);
        }
    }

    private void HandleMoveKey()
    {
        var items = GetPanelItems(currentDirectory);
        var selectedFileForMove = currentDirectory.GetFiles().FirstOrDefault(f => f.Name == items[activeIndex].Name);
        if (selectedFileForMove != null)
        {
            var otherPanelDirectory = OtherPanel.currentDirectory;
            var newFileName = Path.Combine(OtherPanel.currentDirectory.FullName, selectedFileForMove.Name);
            File.Move(selectedFileForMove.FullName, newFileName);
            Console.Clear();
        }
    }

    private void HandleDecryptFile()
    {
        var items = GetPanelItems(currentDirectory);
        var selectedFileForDecrypt = currentDirectory.GetFiles().FirstOrDefault(f => f.Name == items[activeIndex].Name);
        if (selectedFileForDecrypt != null)
        {
            if (!selectedFileForDecrypt.Name.EndsWith(".enc"))
            {
                return;
            }

            // Тут можна додати код для дешифрування файлу
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes key
                aes.IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes key

                using (FileStream fs = new FileStream(selectedFileForDecrypt.FullName, FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (FileStream fsOut = new FileStream(selectedFileForDecrypt.FullName.Replace(".enc", ""), FileMode.Create))
                        {
                            cs.CopyTo(fsOut);
                        }
                    }
                }
            }
        }
    }


    private void Encrypt(FileInfo selectedFileForEncrypt)
    {
        // Тут можна додати код для шифрування файлу
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes key
            aes.IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes key
                                                                 // text.txt => fsIn
            using (FileStream fsIn = new FileStream(selectedFileForEncrypt.FullName, FileMode.Open))
            {
                using (CryptoStream cs = new CryptoStream(fsIn, aes.CreateEncryptor(), CryptoStreamMode.Read))
                {
                    // fsOut => text.txt.enc
                    using (FileStream fsOut = new FileStream(selectedFileForEncrypt.FullName + ".enc", FileMode.Create))
                    {
                        // text.txt => fsIn => CryptoStream => aes.CreateEncryptor => fsOut => text.txt.enc
                        cs.CopyTo(fsOut);
                    }
                }
            }
        }
    }

    private void HandleEncryptFile()
    {
        var items = GetPanelItems(currentDirectory);
        var selectedFileForEncrypt = currentDirectory.GetFiles().FirstOrDefault(f => f.Name == items[activeIndex].Name);

        // file
        if (selectedFileForEncrypt != null)
        {
            Encrypt(selectedFileForEncrypt);
        }


        // directory
        var selectedDirForEncrypt = currentDirectory.GetDirectories().FirstOrDefault(d => d.Name == items[activeIndex].Name);
        if (selectedDirForEncrypt != null)
        {

            var files = GetAllFiles(selectedDirForEncrypt).Where(x => !x.EndsWith(".enc"));
            var queue = new Queue<string>(files);

            var taskPanel = new TaskInfoPanel()
            {
                Title = "Encode files",
                Width = 31,
                Height = 8,
            };
            taskPanel.Draw();

            var progressBar = new ProgressBar() { Value = 0, startY = 7, startX = 30 };
            progressBar.Draw();

            var filesCount = queue.Count;

            Thread t = new Thread(() =>
            {
                //   [] [] [] [] [] [] [] => []

                do
                {
                    var fileName = queue.Dequeue();
                    Encrypt(new FileInfo(fileName));

                    var value = (int)((float)(filesCount - queue.Count) / filesCount * 100);
                    progressBar.Value = value;
                    progressBar.Draw();

                    Thread.Sleep(1000);

                } while (queue.Count > 0);

                progressBar.Clear();
                taskPanel.Clear();
            });

            t.Start();
            t.Join();
        }
    }

    private List<string> GetAllFiles(DirectoryInfo directory)
    {
        List<string> list = new();
        foreach (var d in directory.GetDirectories())
        {
            list.AddRange(GetAllFiles(d));
        }
        foreach (var f in directory.GetFiles())
        {
            list.Add(f.FullName);
        }
        return list;
    }
}
