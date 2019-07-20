using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JsonParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../TestCases/";
            var files = Directory.GetFiles(path);
            foreach(var file in files)
            {
                if(file.EndsWith(".json"))
                {
                    string json = File.ReadAllText(file, Encoding.UTF8);
                    Console.WriteLine(AillieoUtils.JsonParser.Parse(json));
                }
            }

            Console.WriteLine("Finish!");
            Console.ReadKey();
        }
    }
}
