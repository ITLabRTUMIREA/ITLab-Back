using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEnd.Controllers
{
    [Route("api/summary")]
    public class SummaryController : Controller
    {
        private readonly DataBaseContext dbContext;

        public SummaryController(DataBaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var str = new StringBuilder("Фамилия,Имя,");
            var allrecords = await dbContext
                .Users
                .SelectMany(u => u.PlaceUserEventRoles)
                .Include(puer => puer.Place)
                    .ThenInclude(p => p.Shift)
                        .ThenInclude(s => s.Event)
                .Include(puer => puer.User)
                .Include(puer => puer.EventRole)
                .ToListAsync();

            var events = allrecords
                .Select(r => r.Place)
                .Select(r => r.Shift)
                .Distinct()
                .OrderBy(s => s.BeginTime)
                //.Take(10)
                ;
            foreach (var shift in events)
            {
                str.Append($"{shift.Event.Title.Replace(",", ";")},");
            }
            str.Append("\n###,###,");
            foreach (var shift in events)
            {
                str.Append($"{shift.BeginTime.ToString("dd.MM.yyyy")},");
            }
            str.Append('\n');

            foreach (var user in allrecords
                .Select(r => r.User)
                .Distinct()
                .OrderBy(u => u.LastName))
            {
                str.Append($"{user.LastName},{user.FirstName},");
                foreach (var shift in events)
                {
                    var puers = shift.Places.SelectMany(p => p.PlaceUserEventRoles).Where(puer => puer.UserId == user.Id).ToList();
                    if (puers.Count == 0)
                        str.Append("-,");
                    else
                    {
                        var role = puers.Select(puer => puer.EventRole.Title).Single();
                        str.Append($"{role[0]},");
                    }
                }
                str.Append('\n');
            }
            var content = Encoding.UTF8.GetBytes(str.ToString());
            return File(content, "text/csv", "summary.csv");
        }
    }
}
