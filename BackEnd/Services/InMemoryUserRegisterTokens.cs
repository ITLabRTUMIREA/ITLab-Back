using BackEnd.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Options;
using BackEnd.Models;

namespace BackEnd.Services
{
    public class InMemoryUserRegisterTokens : IUserRegisterTokens
    {
        private const string availableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random random = new Random();
        private readonly Dictionary<string, string> tokensStorage = new Dictionary<string, string>();
        public InMemoryUserRegisterTokens(IOptions<List<RegisterTokenPair>> defaults)
        {
            if (defaults.Value == null)
                return;
            foreach (var pair in defaults.Value) 
                tokensStorage[pair.Email] = pair.Token;
        }
        public string AddRegisterToken(string email)
        {
            var randString = RandomString(20);
            tokensStorage[email] = randString;
            return randString;
        }

        public bool IsCorrectToken(string email, string token)
            => tokensStorage.TryGetValue(email, out var fromStorageToken) && fromStorageToken == token;

        public void RemoveToken(string email)
            => tokensStorage.Remove(email);

        private string RandomString(int length)
            => new string(Enumerable.Repeat(availableChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
