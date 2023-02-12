using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace track_editor_console
{

    public class ConsoleHelper
    {
        public static int MultipleChoice(bool canCancel, string menuText, params string[] options)
        {
            const int startX = 2;
            const int startY = 2;
            const int optionsPerLine = 1;
            const int spacingPerLine = 14;

            int currentSelection = 0;

            ConsoleKey key;

            Console.CursorVisible = false;

            do
            {
                Console.Clear();

                Console.SetCursorPosition(startX, startY-1);

                Console.WriteLine(menuText);

                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine, startY + i / optionsPerLine);

                    if (i == currentSelection)
                        Console.ForegroundColor = ConsoleColor.Cyan;

                    Console.Write(options[i]);

                    Console.ResetColor();
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (currentSelection % optionsPerLine > 0)
                                currentSelection--;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (currentSelection % optionsPerLine < optionsPerLine - 1)
                                currentSelection++;
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (currentSelection >= optionsPerLine)
                                currentSelection -= optionsPerLine;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (currentSelection + optionsPerLine < options.Length)
                                currentSelection += optionsPerLine;
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            if (canCancel)
                                return -1;
                            break;
                        }
                }
            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            return currentSelection;
        }
    }

    class Game
    {
        private string Language;
        private char[,] Map;
        private List<string> menuItems;

        public char[,] GetMap()
        {
            return this.Map;
        }

        public void SetMap(char[,] newMap) {
            this.Map = newMap;
        }

        public static List<string> loadLanguageData(string languageFileName)
        {

            List<string> data = new List<string>();

            try
            {
                StreamReader sr = new StreamReader($"../../{languageFileName}");

                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    data.Add(line);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return data;
        }

        public void StartGame()
        {
            int selectedLanguage = ConsoleHelper.MultipleChoice(true, "Válasszon nyelvet / Choose a language:", "Magyar", "English");

            switch (selectedLanguage)
            {
                case 0:
                    this.menuItems = loadLanguageData("LanguageFiles/hungarianMainMenu.txt");
                    this.Language = "hungarian";
                    break;
                case 1:
                    this.menuItems = loadLanguageData("LanguageFiles/englishMainMenu.txt");
                    this.Language = "english";
                    break;
                default:
                    Console.WriteLine("Hiba történt! Próbálja újra!");
                    StartGame();
                    break;
            }
            MainMenu();

        }

        public void MainMenu()
        {
            string mainMenuText = this.menuItems[0];
            int selectedMenu = ConsoleHelper.MultipleChoice(true, mainMenuText, menuItems.Skip(1).ToArray());
            switch (selectedMenu)
            {
                case 0:
                    this.LoadMap();
                    break;
                case 1:
                    this.PlaceMap(true);
                    break;
                case 2:
                    this.StartGame();
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Hiba történt! Próbálja újra!");
                    StartGame();
                    break;
            }
        }

        public char[,] LoadMapFromFile(string filePath)
        {

            string[] numberOfRows = File.ReadAllLines(filePath);
            char[,] map = new char[numberOfRows.Length, numberOfRows[0].Length];
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    map[row, col] = numberOfRows[row][col];
                }
            }
            return map;
        }

        public void LoadMap()
        {
            List<string> loadMapMenuItems = this.Language == "hungarian" ? loadLanguageData("LanguageFiles/loadMapHungarian.txt") : loadLanguageData("LanguageFiles/loadMapEnglish.txt");
            string loadError = this.Language == "hungarian" ? "\n\nHiba történt! A file vagy rossz formátumú vagy üres! Kérem másik pályát válasszon ki!" : "\n\nAn error occured! The file is in the wrong format or empty! Please choose another map!";

            List<string> mapFilePaths = Directory.GetFiles("../../Maps", "*.txt", SearchOption.AllDirectories).ToList();
            List<string> mapFiles = new List<string>();
            foreach(string mapFilePath in mapFilePaths)
            {
                mapFiles.Add(Path.GetFileName(mapFilePath));
            }
            mapFiles.Add(loadMapMenuItems.ToArray()[loadMapMenuItems.Count - 1]);

            int mapMenu = ConsoleHelper.MultipleChoice(false, loadMapMenuItems[0], mapFiles.ToArray());
            if (mapMenu == mapFiles.Count - 1)
            {
                MainMenu();
            }

            try
            {
                char[,] loadedMap = LoadMapFromFile(mapFilePaths[mapMenu]);
                this.SetMap(loadedMap);
                PlaceMap(false, 0, 0);
            }
            catch (Exception)
            {
                Console.WriteLine(loadError);
                Console.ReadKey();
                LoadMap();
            }

        }

        public void PlaceMap(bool isNewMap, int startPositionX=0, int startPositionY=0)
        {
            Console.Clear();
            if (!isNewMap)
            {
                for (int i = 0; i < this.GetMap().GetLength(0); i++)
                {
                    for (int j = 0; j < this.GetMap().GetLength(1); j++)
                    {
                        Console.Write(this.Map[i, j]);
                    }
                    Console.WriteLine();
                }

                this.EditMap(startPositionX, startPositionY, true);

            }
            else
            {
                List<string> newMapMessages = this.Language == "hungarian" ? loadLanguageData("LanguageFiles/newMapHungarian.txt") : loadLanguageData("LanguageFiles/newMapEnglish.txt");

                try
                {    
                    Console.Write(newMapMessages[1]);
                    int height = int.Parse(Console.ReadLine());
                    Console.Write(newMapMessages[0]);
                    int width = int.Parse(Console.ReadLine());
                    Console.Clear();
                    if (width < 10 || height < 10)
                    {
                        Console.WriteLine(newMapMessages[2]);
                        PlaceMap(true, 0, 0);
                    }
                    else
                    {
                        char[,] newMap = new char[height, width];

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                newMap[i, j] = '.';
                                Console.Write(".");
                            }
                            Console.WriteLine();
                        }

                        this.SetMap(newMap);
                        this.EditMap(0, 0, true);
                        
                    }

                }
                catch (Exception)
                {

                    Console.WriteLine(newMapMessages[2]);
                    PlaceMap(true, 0, 0);
                }
            }
        }

        public void EditMap(int editPositionX, int editPositionY, bool isInstructions)
        {
            List<string> editInstructions = this.Language == "hungarian" ? loadLanguageData("LanguageFiles/editMapHungarian.txt") : editInstructions = loadLanguageData("LanguageFiles/editMapEnglish.txt");

            if (isInstructions)
            {
                Console.WriteLine();
                for (int i = 0; i < editInstructions.Count; i++)
                {
                    Console.Write(editInstructions[i] + "\t");
                    if(i % 2 == 1)
                    {
                        Console.WriteLine("\n");
                    }
                }
            }

            ConsoleKey key;


            Console.SetCursorPosition(editPositionX, editPositionY);

            do
            {

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                    {
                        if(editPositionX == 0)
                        {
                            editPositionX = 0;
                        }
                        else
                        {
                            editPositionX--;
                        }
                        EditMap(editPositionX, editPositionY, false);
                        break;
                        }
                    case ConsoleKey.RightArrow:
                    {
                        if(editPositionX == this.GetMap().GetLength(1) - 1)
                        {
                            editPositionX = this.GetMap().GetLength(1) - 1;
                        }
                        else
                        {
                            editPositionX++;
                        }
                        EditMap(editPositionX, editPositionY, false);
                        break;
                    }
                    case ConsoleKey.UpArrow:
                    {
                        if (editPositionY == 0)
                        {
                            editPositionY = 0;
                        }
                        else
                        {
                            editPositionY--;
                        }
                        EditMap(editPositionX, editPositionY, false);
                        break;
                    }
                    case ConsoleKey.DownArrow:
                    {
                        if (editPositionY == this.GetMap().GetLength(0) - 1)
                        {
                            editPositionY = this.GetMap().GetLength(0) - 1;
                        }
                        else
                        {
                            editPositionY++;
                        }
                        EditMap(editPositionX, editPositionY, false);
                        break;
                    }
                    case ConsoleKey.Delete:
                        {
                            ReplaceMapCharacter('.', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F1:
                        {
                            ReplaceMapCharacter('█', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F2:
                        {
                            ReplaceMapCharacter('═', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F3:
                        {
                            ReplaceMapCharacter('║', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F4:
                        {
                            ReplaceMapCharacter('╬', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F5:
                        {
                            ReplaceMapCharacter('╦', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F6:
                        {
                            ReplaceMapCharacter('╩', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F7:
                        {
                            ReplaceMapCharacter('╣', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F8:
                        {
                            ReplaceMapCharacter('╠', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F9:
                        {
                            ReplaceMapCharacter('╗', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F10:
                        {
                            ReplaceMapCharacter('╝', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F11:
                        {
                            ReplaceMapCharacter('╚', editPositionX, editPositionY);
                            break;
                        }
                    case ConsoleKey.F12:
                    {
                        SaveMap();
                        break;
                    }
                    case ConsoleKey.Enter:
                        {
                            ChangeTrackSizes();
                            break;
                        }
                }
            } while (true);

        }

        public void ReplaceMapCharacter(char character, int editPositionX, int editPositionY)
        {
            this.Map[editPositionY, editPositionX] = character;
            PlaceMap(false, editPositionX, editPositionY);
        }

        public void SaveMap()
        {
            List<string> saveMenus = this.Language == "hungarian" ? loadLanguageData("LanguageFiles/saveMapHungarian.txt") : loadLanguageData("LanguageFiles/saveMapEnglish.txt");
            string askSave = this.Language == "hungarian" ? "Adja meg a file nevét! Figyelem! Ha a megadott file létezik, akkor a tartalmát felül fogja írni!" : "Enter the name of the file! Warning! If the entered file exists, its content will be overwritten!";
            string successfullySave = this.Language == "hungarian" ? "Sikeres mentés! Nyomjon meg egy gombot a továbblépéshez!" : "Save success! Press any button to continue.";
            int selectedMenu = ConsoleHelper.MultipleChoice(true, saveMenus[0], saveMenus.Skip(1).ToArray());
            Console.WriteLine(selectedMenu);
            switch (selectedMenu)
            {
                case 0:
                    {
                        if(getNumberOfRooms() > 0 && getNumberOfExits() > 0)
                        {
                            Console.Clear();
                            Console.WriteLine(askSave);
                            Console.Write("File: ");
                            string fileName = Console.ReadLine() + ".txt";
                        
                            StreamWriter sr = new StreamWriter($"../../Maps/{fileName}");
                            for (int col = 0; col < GetMap().GetLength(0); col++)
                            {
                                for (int row = 0; row < GetMap().GetLength(1); row++)
                                {
                                    sr.Write(GetMap()[col, row]);
                                }
                                sr.WriteLine();
                            }
                            sr.Close();
                            Console.WriteLine(successfullySave);
                            Console.ReadKey();
                            MainMenu();
                        }
                        else
                        {
                            Console.Clear();
                            if (this.Language == "hungarian")
                            {
                                Console.WriteLine("Nem mentheti el a pályát szoba vagy kijárat nélkül!");
                            }
                            else
                            {
                                Console.WriteLine("You mustn't save the track without any room or exit!");
                            }
                            Console.ReadKey();
                            PlaceMap(false);
                        }
                        break;
                    }
                case 1:
                    {
                        MainMenu();
                        SetMap(new char[0,0]);
                        break;
                    }
                case 2:
                    {
                        PlaceMap(false);
                        break;
                    }
            }
        }

        public void ChangeTrackSizes()
        {
            Console.Clear();
            List<string> trackSizeMessages = this.Language == "hungarian" ? loadLanguageData("LanguageFiles/changeTrackSizeHungarian.txt") : loadLanguageData("LanguageFiles/changeTrackSizeEnglish.txt");
            Console.WriteLine(trackSizeMessages[0]);
            try
            {
                Console.Write(trackSizeMessages[1]);
                int newWidth = Convert.ToInt32(Console.ReadLine());
                Console.Write(trackSizeMessages[2]);
                int newHeight = Convert.ToInt32(Console.ReadLine());
                if (newWidth == 0 && newHeight == 0)
                {
                    PlaceMap(false, 0, 0);
                }
                else if (newWidth < 10 || newHeight < 10)
                {
                    Console.Clear();
                    Console.WriteLine(trackSizeMessages[3]);
                    Console.ReadKey();
                    ChangeTrackSizes();
                }

                char[,] newMap = new char[newWidth, newHeight];
                for (int col = 0; col < newWidth; col++)
                {
                    for (int row = 0; row < newHeight; row++)
                    {
                        try
                        {
                            newMap[col, row] = GetMap()[col, row];
                        }
                        catch (Exception)
                        {
                            newMap[col, row] = '.'; 
                        }
                    }
                }
                this.SetMap(newMap);
                PlaceMap(false, 0, 0);

            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine(trackSizeMessages[3]);
                ChangeTrackSizes();
            }
        }

        public int getNumberOfRooms()
        {
            int numberOfRooms = 0;
            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (Map[i, j] == '█')
                    {
                        numberOfRooms++;
                    }
                }
            }

            return numberOfRooms;
        }

        public int getNumberOfExits()
        {
            int numberOfExits = 0;
            List<char> firstRowChars = new List<char>()
            {
                '╬' ,'╩','║','╣','╠', '╝','╚'
            };

            List<char> lastRowChars = new List<char>()
            {
                '╬','╦','║','╣','╠','╗', '╔'
            };
            
            List<char> firstColChars = new List<char>()
            {
                '╬','═','╦','╩','╣','╗','╝',
            };
            
            List<char> lastColChars = new List<char>()
            {
                '╬','═','╦','╩','╠','╚', '╔'
            };

            for (int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    if (firstRowChars.Contains(Map[0, j]) || lastRowChars.Contains(Map[Map.GetLength(0) - 1, j]) || firstColChars.Contains(Map[i, 0]) || lastColChars.Contains(Map[i, Map.GetLength(1) - 1]))
                    {
                        numberOfExits++;
                    }
                }
            }

            return numberOfExits;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.StartGame();
        }
    }
}
