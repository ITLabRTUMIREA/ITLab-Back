using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ApiSchemaCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Debug", "netcoreapp2.0");
            var start = "/Users/maksim/Desktop/";
            var targetPath = Path.Combine(start, "publish");

            Console.WriteLine("Hello World! from dotnet cli");
            //var code = new ProjectBuilder(Path.Combine("/Users/maksim/Desktop", "publish")).Build();
            //Console.WriteLine($"Exit code {code}");
            
            var allTypes =
                Directory
                .GetFiles(targetPath)
                .Where(f => Path.GetExtension(f) == ".dll")
                .Reverse()
                .Select(f => { Console.WriteLine(f); return f; })
                .SelectMany(GetTypes)
                .ToList();
            allTypes.ForEach(type => Console.WriteLine(type));
            Console.Read();
        }

        static Type[] GetTypes(string path) 
        {
            try
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(path).GetTypes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------------" + ex.Message);
                return new Type[0];
            }

        }
    }

}
