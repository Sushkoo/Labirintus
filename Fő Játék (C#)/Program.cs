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
        /*
        Use it Later
        public Dictionary<char,string[]> routeDict= new()
        {
            {'═',new string[]{"Nyugat","Kelet"}},
        };
        */
        public delegate void CenterText();
        public static CenterText ct = () => Console.SetCursorPosition(Console.WindowWidth / 2 - 30, 0);
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
            do
            {
                ct();
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
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }

        ///<summary>Nyelvválasztó menü</summary>
        public static void Language()
        {
            Console.Clear();
            ct();
            Console.WriteLine("----- Válaszd Ki a nyelvet -----");
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
            ct();
            Console.WriteLine("----Választható pályák-----");
            DirectoryInfo d = new(@"./Maps");
            FileInfo[] maps = d.GetFiles("*.txt");
            for (int count = 0; count < maps.Length; count++)
            {
                System.Console.WriteLine($"[{count + 1}]. {maps[count].Name}");
            }
            Console.Write("Írd be a pálya nevét. A Főmenübe való visszalpéshez nyomd meg az ENTER billentyűt: ");
            string answer = Console.ReadLine()!;
            while (maps.All(x => answer != x.Name))
            {
                if (answer == string.Empty)
                {
                    MainMenu();
                }
                Console.Write("\rHiba. Rossz pályanevet adtál meg! Ha ki akarsz lépni, nyomj meg egy ENTER-t: ");
                answer = Console.ReadLine()!;

            }


            Console.WriteLine("Válaszd ki a nehézséget\n [1]. Normál\t\t[2.]Nehéz\t\t[3]Vak mód.");
            int diff = Math.Clamp(int.Parse(Console.ReadLine()!), 1, 3);
            Maze maze = new Maze(answer, diff);
            maze.CreateMap(ref answer);
            maze.ShowMap();
            Player player = new Player(1, 0,maze);
            player.Move();


        }

    }



    //Osztály a labirintushoz
    public class Maze
    {
        //TODO
        /*
            - Kijáratok, Termek megszámlálása
            - Játékos pozicióinak elraktározása, ellenőrzése
            - Játékos talált-e kincset
                - Ha talált akkor ki írja
            - Kilépés a labirintusból
            - Játék közbeni mentés

        */
        private char[,]? map;
        public string? FileName { get; set; }

        public int? Difficulty { get; set; }

        public Maze(string fname, int diff)
        {
            FileName = fname;
            Difficulty = diff;
        }
        internal void CreateMap(ref string filename)
        {
            string[] file = File.ReadAllLines($".//Maps//{filename}");
            map = new char[file.Length, file[0].Length];
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    map[row, col] = file[row][col];
                }
            }
        }
        public void ShowMap()
        {
                Console.Clear();
                Console.WriteLine(this.ToString());
                for (int row = 0; row < map!.GetLength(0); row++)
                {
                    for (int col = 0; col < map.GetLength(1); col++)
                    {
                        Console.Write(map[row, col]);
                    }
                    Console.WriteLine();
                }
                
            
        }
        public override string ToString()
        {
            return $"Pálya neve: {this.FileName}, mérete: {this.map!.GetLength(0)} sor x {this.map.GetLength(1)} oszlop.";
        }


    }
    public class Player
    {
        /*
            
        */
        private Maze Mz;
        const char CHARACTER_SPRITE = '▒';
        public int posX { get; set; }
        public int posY { get; set; }
        public Player(int x, int y, Maze mz)
        {
            posX = x;
            posY = y;
            this.Mz = mz;
        }
        public void Move()
        {
            Console.CursorVisible = false;
            
            while (true)
            {
                
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        posY--;
                        break;
                    case ConsoleKey.S:
                        posY++;
                        break;
                    case ConsoleKey.A:
                        posX--;
                        break;
                    case ConsoleKey.D:
                        posX++;
                        break;

                }
                Mz.ShowMap();
                Console.SetCursorPosition(posX, posY);
                Console.Write(CHARACTER_SPRITE);
                Console.SetCursorPosition(0,Console.WindowHeight-3);
                Console.Write($"X: {this.posX}\n Y: {this.posY}");
                
            }
        }
    }
}

