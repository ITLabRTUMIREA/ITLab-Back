using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Settings
{
    public enum DbType
    {
        SQL_SERVER_REMOTE,
        SQL_SERVER_LOCAL,
        IN_MEMORY,
        POSTGRES_LOCAL
    }
}
