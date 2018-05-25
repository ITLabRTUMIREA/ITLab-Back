using System;
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
            Console.WriteLine("Hello World! from dotnet cli");
            var code = new ProjectBuilder().Build();
            Console.WriteLine($"Exit code {code}");
            
            //var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Debug", "netcoreapp2.0");
            var targetPath = Path.Combine(@"C:\Users\maksa\source\repos\LaboratoryWorkControl\BackEnd", "bin", "Debug", "netcoreapp2.0", "publish");
            var allTypes =
                Directory
                .GetFiles(targetPath)
                .Where(f => Path.GetExtension(f) == ".dll")
                .Reverse()
                .Select(f => { Console.WriteLine(f); return f; })
                .SelectMany(f => AssemblyLoadContext.Default.LoadFromAssemblyPath(f).GetTypes())
                .ToList();
            allTypes.ForEach(type => Console.WriteLine(type));
            Console.Read();
        }
    }
}
