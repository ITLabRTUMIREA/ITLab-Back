using System;
using BackEnd.Services.Interfaces;
using BackEnd.DataBase;
using BackEnd.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BackEnd.Services
{
    public class DbUserRegisterTokens : IUserRegisterTokens
    {
        private readonly DataBaseContext dbContext;
        private readonly List<RegisterTokenPair> defaultPairs;
        private const string availableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random random = new Random();
        private Expression<Func<RegisterTokenPair, bool>> EqualsChecker(string email, string token)
            => (RegisterTokenPair rtp) => rtp.Email == email && rtp.Token == token;

        public DbUserRegisterTokens(
            DataBaseContext dbContext,
            IOptions<List<RegisterTokenPair>> defaultPairs)
        {
            this.dbContext = dbContext;
            this.defaultPairs = defaultPairs.Value;
        }

        public async Task<string> AddRegisterToken(string email)
        {
            var code = RandomString(20);
            dbContext.RegisterTokenPairs.Add(
                new RegisterTokenPair
                {
                    Email = email,
                    Token = code
                });
            await dbContext.SaveChangesAsync();
            return code;
        }

        public async Task<bool> IsCorrectToken(string email, string token)
            => defaultPairs.Any(EqualsChecker(email, token).Compile()) 
                           || await dbContext.RegisterTokenPairs.AnyAsync(EqualsChecker(email, token));

        public async Task RemoveToken(string email)
        {
            var targetSet = await dbContext
                .RegisterTokenPairs
                .Where(rtp => rtp.Email == email)
                .ToListAsync();
            dbContext.RemoveRange(targetSet);
            await dbContext.SaveChangesAsync();
        }

        private string RandomString(int length)
            => new string(Enumerable.Repeat(availableChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
