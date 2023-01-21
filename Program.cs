using System.IO;
using System.Linq.Expressions;
using GBX.NET;
using GBX.NET.Engines.Game;
using GBX.NET.LZO;

namespace TFLH
{
    class Program
    {
        static Dictionary<string, string> MapUIDs = new Dictionary<string, string>();

        static void showHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\n-h / --help\t\t\t\tto view this info");
            Console.WriteLine("\n-f / --find FileList.Gbx MapName\tKeep in mind the first maps are internally named 001 or 087.");
            Console.WriteLine("\n-f / --find MapName\t\t\tUse this if you have FileList.Gbx in the same directory as the application.");
            Console.WriteLine("\n-e / --extract folder\t\t\tPass a folder with Turbo maps to add to the dictionary.");
        }

        static void readMapDict(string csvFile)
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader(csvFile);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                File.Create("mapDict.csv").Close();
                sr = new StreamReader("mapDict.csv");
            }

            string? line;
            while((line = sr.ReadLine()) is not null)
            {
                string[] row = line.Split(',', 2);
                if(row.Length > 1)
                    MapUIDs.Add(row[0], row[1]);
            }
            sr.Close();
        }

        static void extractMaps(string folder)
        {
            readMapDict("mapDict.csv");

            uint addCount = 0;

            GBX.NET.Lzo.SetLzo(typeof(GBX.NET.LZO.MiniLZO));
            foreach(string filename in Directory.EnumerateFiles(folder, "*.Map.Gbx"))
            {
                var node = GameBox.ParseNode(filename);
                if (node is CGameCtnChallenge map)
                {
                    if (MapUIDs.TryAdd(map.MapName, map.MapUid))
                        addCount++;
                }
            };

            string[] lines = new string[MapUIDs.Count];
            uint i = 0;
            foreach(KeyValuePair<string, string> kvp in MapUIDs)
            {
                lines[i++] = kvp.Key + ',' + kvp.Value;
            };

            File.WriteAllLines("mapDict.csv", lines);

            Console.WriteLine("Added " + addCount + " maps to the dictionary. New total dictionary size: " + MapUIDs.Count);
        }

        static void findGhost(string filename, string MapName)
        {
            Console.WriteLine("Searching for files with data related to the map " + MapName + ".\n");

            GBX.NET.Lzo.SetLzo(typeof(GBX.NET.LZO.MiniLZO));
            var node = GameBox.ParseNode(filename);
            if (node is CGameUserFileList fileList)
            {
                readMapDict("mapDict.csv");

                foreach (CGameUserFileList.FileInfo fileInfo in fileList.Files.Where(f => f.Type == CGameUserFileList.FileType.Map))
                {
                    if (fileInfo.MapName == MapName)
                    {
                        MapUIDs.Add(fileInfo.MapName, fileInfo.MapUid);
                        Console.WriteLine("Map found: " + fileInfo.Name);
                    }
                }

                string? MapUID = "";
                if (MapUIDs.TryGetValue(MapName, out MapUID))
                {
                    foreach (CGameUserFileList.FileInfo fileInfo in fileList.Files.Where(f => f.Type == CGameUserFileList.FileType.Ghost))
                    {
                        if (fileInfo.MapUid == MapUID)
                        {
                            Console.WriteLine("Ghost (" + fileInfo.GhostKind + ") found: " + fileInfo.Name);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("The given map name " + MapName + " could not be found in the dictionary. Check the spelling or run the --extract command on a folder of maps to add them.");
                    Console.WriteLine("If you want to add a map by hand you can add a new line in mapDict.csv formatted as");
                    Console.WriteLine("\nmapName,mapUID\t\t(no spaces next to the comma, otherwise these will be part of the name or UID respectively)");
                }
            }
            else
            {
                Console.WriteLine("The given file is not a CGameUserFileList.");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("\nTurboFileListHelper v0.1 by Mystixor\n\n");

            if (args.Length < 2 || args[0] == "-h" || args[0] == "--help")
            {
                showHelp();
                return;
            }

            if (args.Length == 3 && (args[0] == "-f" || args[0] == "--find"))
                findGhost(args[1], args[2]);
            else if (args.Length == 2 && (args[0] == "-f" || args[0] == "--find"))
                try
                {
                    findGhost("FileList.Gbx", args[1]);
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    if(ex.FileName.EndsWith("FileList.Gbx"))
                        Console.WriteLine("Could not find FileList.Gbx in the same directory as the application.");
                }
            else if (args.Length == 2 && (args[0] == "-e" || args[0] == "--extract"))
                extractMaps(args[1]);
            else
                showHelp();

            return;
        }
    }
}