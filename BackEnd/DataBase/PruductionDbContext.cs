using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.DataBase
{
    public class ProductionDbContext : DataBaseContext
    {
        public ProductionDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
