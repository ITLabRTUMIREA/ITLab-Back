using System;
using System.Threading.Tasks;
using WebApp.Configure.Models.Configure.Interfaces;

namespace BackEnd.Services.ConfigureServices
{
    public class DBInitService : IConfigureWork
    {


        public async Task Configure()
        {
            for (int i = 20; i > 0; i--)
            {
                Console.WriteLine($"{i} LEFT!!@!:@:@:@@@!!!!");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
