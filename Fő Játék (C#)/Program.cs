using System;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Linq;
namespace Menu
{

    class Program
    {
        public static CultureInfo cinfo = Thread.CurrentThread.CurrentCulture;
        //public static string language = "hu-HU"; ONHOLD

        static void Main(string[] args)
        {

            //Clear the Console Screen
            Console.Clear();
            //Fire up the Main Menu
            MainMenu();

        }
        ///<summary>Főmenü</summary>
        public static void MainMenu()
        {


            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2 - 30, 0);
            Console.WriteLine("-----Labirintus-----\n");
            Console.WriteLine($"Jelenlegi nyelv: {cinfo}");
            Console.WriteLine("[1]. Nyelv kiválasztása");
            Console.WriteLine("[2]. Pálya betöltése");
            Console.WriteLine("[3]. Pályaszerkesztő");
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.KeyChar)
            {
                case '1':
                    Language();
                    break;
                case '2':
                    LoadMap();
                    break;
            }

        }

        ///<summary>Nyelvválasztó menü</summary>
        public static void Language()
        {
            Console.Clear();
            Console.WriteLine($"Jelenlegi nyelv: {cinfo}");
            Console.WriteLine("[1]. Magyar");
            Console.WriteLine("[2]. Angol");
            Console.WriteLine("[3]. Kilépés a főmenübe");
            switch (Console.ReadKey(true).KeyChar)
            {
                case '1':
                    cinfo = new CultureInfo("hu-HU", true);
                    MainMenu();
                    break;
                case '2':
                    cinfo = new CultureInfo("en-EN", true);
                    MainMenu();
                    break;
                case '3':
                    MainMenu();
                    break;

            }

        }
        ///<summary>Pályaválasztás (OPTIMALIZÁLÁS SZÜKSÉGES)</summary>
        public static void LoadMap()
        {
            
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2 - 30, 0);
            Console.WriteLine("----Választható pályák-----");
            DirectoryInfo d = new(@"./Maps");
            FileInfo[] maps = d.GetFiles("*.txt");
            for (int count = 0; count < maps.Length; count++)
            {
                System.Console.WriteLine($"[{count + 1}]. {maps[count].Name}");
            }
            Console.Write("Írd be a pálya nevét. A Főmenübe való visszalpéshez nyomd meg az ENTER billentyűt: ");
            string answer = Console.ReadLine()!;
            if (answer == string.Empty)
            {
                MainMenu();
            }
            else
            {
                //Check if answer is in maps
                while (maps.All((x) => x.Name != answer))
                {
                    Console.Write("\nHiba. Rossz pályanevet adtál meg,: ");
                    answer = Console.ReadLine()!;
                }
            }

            Console.WriteLine("Válaszd ki a nehézséget\n [1]. Normál\t\t[2.]Nehéz\t\t[3]Vak mód.");
            int diff = Math.Clamp(int.Parse(Console.ReadLine()!),1,3);
            Maze maze = new Maze(answer,diff);


        }

    }

    public class Maze 
    {
        //TODO
        /*
            - Fájból kiolvasás, Array betöltése
            - Kijáratok, Termek megszámlálása
            - Játékos lehelyezése

        */
        private char[,]? map;
        public string? FileName {get;set;}
        
        public int? Difficulty {get;set;}

        public Maze(string fname, int diff) 
        {
            FileName = fname;
            Difficulty = diff;
            CreateMap(FileName);
            ShowMap();
        }
        private void CreateMap(string filename) 
        {
            Console.Clear();
            string[] file = File.ReadAllLines($".//Maps//{filename}");
            map = new char[file.Length,file[0].Length];
            for(int row = 0; row < map.GetLength(0);row++) 
            {
                for(int col = 0; col < map.GetLength(1);col++) 
                {
                    map[row,col] = file[row][col];
                }
            }
        }
        public void ShowMap() 
        {
              for(int row = 0; row < map!.GetLength(0);row++) 
            {
                for(int col = 0; col < map.GetLength(1);col++) 
                {
                    Console.Write(map[row,col]);
                }
                Console.WriteLine();
            }
        }

    }
}

