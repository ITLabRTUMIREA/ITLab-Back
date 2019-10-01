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
        private const string AvailableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random Random = new Random();
        private static Expression<Func<RegisterTokenPair, bool>> EqualsChecker(string email, string token)
        {
            return rtp => rtp.Email == email && rtp.Token == token;
        }

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

        public async Task<bool> IsCorrectRegisterToken(string email, string token)
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

        private static readonly HashSet<(Guid id, string token)> inMemoryVkTokens = new HashSet<(Guid id, string token)>();

        public Task<string> AddVkToken(Guid userId)
        {
            var token = RandomString(20);
            inMemoryVkTokens.Add((userId, token));
            return Task.FromResult(token);
        }

        public Task<Guid?> CheckVkToken(string token)
            => Task.FromResult(inMemoryVkTokens.Any(pair => token == pair.token) ? 
                inMemoryVkTokens.FirstOrDefault(pair => token == pair.token).id : default(Guid?));

        private static string RandomString(int length)
            => new string(Enumerable.Repeat(AvailableChars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}
