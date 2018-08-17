using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface IUserRegisterTokens
    {
        Task<string> AddRegisterToken(string email);
        Task<bool> IsCorrectToken(string email, string token);
        Task RemoveToken(string email);
    }
}
