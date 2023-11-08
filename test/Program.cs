using System;
using System.IO;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите диск:");
            DriveInfo[] drives = DriveInfo.GetDrives();

            string[] driveDescriptions = new string[drives.Length];

            for (int i = 0; i < drives.Length; i++)
            {
                driveDescriptions[i] = $"{drives[i].Name} ({drives[i].DriveFormat})  Емкость диска: {drives[i].TotalSize / (1024 * 1024 * 1024)} GB Свободно: {drives[i].AvailableFreeSpace / (1024 * 1024 * 1024)} GB";
            }

            int selectedDriveIndex = ShowMenu(driveDescriptions, 0, driveDescriptions.Length - 1);

            if (selectedDriveIndex == -1)
                return;

            DriveInfo selectedDrive = drives[selectedDriveIndex];
            ShowDirectoryContents(selectedDrive.RootDirectory.FullName);
        }
    }
    static void ShowDirectoryContents(string path)
    {
        while (true)
        {
            try
            {
                string[] directories = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);

                string[] menuItems = new string[directories.Length + files.Length + 3];
                menuItems[0] = string.Format("{0,-50} {1,-19} {2}", "Название", "Дата создания", "     Тип файла");

                // Добавляем разделительную строку
                menuItems[1] = new string('-', 90); // Строка из 80 символов '-'

                for (int i = 0; i < directories.Length; i++)
                {
                    string folderCreationDate = Directory.GetCreationTime(directories[i]).ToString("yyyy-MM-dd HH:mm:ss");
                    menuItems[i + 2] = $"Папка: {Path.GetFileName(directories[i]),-43} {folderCreationDate}      {Path.GetExtension(directories[i])}";
                }

                for (int i = 0; i < files.Length; i++)
                {
                    string fileCreationDate = File.GetCreationTime(files[i]).ToString("yyyy-MM-dd HH:mm:ss");
                    string fileType = "Файл";
                    menuItems[i + directories.Length + 2] = $"{Path.GetFileName(files[i]),-50} {fileCreationDate}      {Path.GetExtension(files[i])}";
                }

                menuItems[menuItems.Length - 2] = "Вернуться в предыдущую папку";
                menuItems[menuItems.Length - 1] = "Вернуться к выбору диска";

                int choice = ShowMenu(menuItems, 0, menuItems.Length - 1);

                if (choice < directories.Length + 2)
                {
                    ShowDirectoryContents(directories[choice - 2]);
                }
                else if (choice < directories.Length + files.Length + 2)
                {
                    string selectedFile = files[choice - directories.Length - 2];
                    OpenFile(selectedFile);
                }
                else if (choice == menuItems.Length - 2)
                {
                    string parentDirectory = Directory.GetParent(path)?.FullName;
                    if (parentDirectory != null)
                    {
                        ShowDirectoryContents(parentDirectory);
                    }
                }
                else if (choice == menuItems.Length - 1)
                {
                    return;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Нет доступа к этой папке.");
                Console.WriteLine("Нажмите любую клавишу для продолжения.");
                Console.ReadKey();
                return;
            }
        }
    }



    static void OpenFile(string filePath)
    {
        string fileExtension = Path.GetExtension(filePath).ToLower();

        try
        {
            if (fileExtension == ".txt")
            {
                Process.Start("notepad.exe", filePath);
            }
            else if (fileExtension == ".docx")
            {
                Process.Start("winword.exe", filePath);
            }
            else
            {
                Console.WriteLine("Невозможно открыть данный тип файла.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при открытии файла: {ex.Message}");
        }

        Console.WriteLine("Нажмите любую клавишу для продолжения.");
        Console.ReadKey();
    }

    static int ShowMenu(string[] menuItems, int startIndex, int endIndex)
    {
        int selectedItem = startIndex;
        while (true)
        {
            Console.Clear();
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i == selectedItem)
                {
                    Console.WriteLine("-> " + menuItems[i]);
                }
                else
                {
                    Console.WriteLine("   " + menuItems[i]);
                }
            }

            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedItem = Math.Max(startIndex, selectedItem - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedItem = Math.Min(endIndex, selectedItem + 1);
                    break;
                case ConsoleKey.Enter:
                    return selectedItem;
                case ConsoleKey.Escape:
                    return -1;
            }
        }
    }
}
