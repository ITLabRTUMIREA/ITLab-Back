using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Interfaces
{
    public interface IUserRegisterTokens
    {
        string AddRegisterToken(string email);
        bool IsCorrectToken(string email, string token);
        void RemoveToken(string email);
    }
}
