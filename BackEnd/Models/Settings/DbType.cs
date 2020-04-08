using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Settings
{
    /// <summary>
    /// Enum, which describe current db type
    /// </summary>
    public enum DbType
    {
        /// <summary>
        /// Use debug in memory database
        /// </summary>
        IN_MEMORY,
        /// <summary>
        /// Use postgres database
        /// </summary>
        POSTGRES
    }
}
