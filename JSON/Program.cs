using System;
using System.IO;
using System.Linq;

namespace JSON
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Count() < 2)
            {
                Console.WriteLine("Usage: jsondiff.exe first.json second.json");
                return 1;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine(args[0] + " does not exist");
                return 2;
            }
            if (!File.Exists(args[1]))
            {
                Console.WriteLine(args[1] + " does not exist");
                return 2;
            }

            try
            {
                string first = File.ReadAllText(args[0]);
                string second = File.ReadAllText(args[1]);

                var diff = JsonDiff.DiffStrings(first, second);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "diff.json");
                File.WriteAllText(path, diff);
                Console.WriteLine("Diff file location: " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("JSON diff failed");
                Console.WriteLine("Ex: " + ex.Message);
                return 3;
            }            

            return 0;
        }
    }
}
