using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.DataBase;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.PublicAPI.Requests.Summary;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

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

        [HttpPost]
        public async Task<IActionResult> GetAsync([FromBody]GetSummaryRequest request)
        {
            var allrecords = await dbContext
                .Users
                .SelectMany(u => u.PlaceUserEventRoles)

                .Where(puer => request.TargetEventTypes.Contains(puer.Place.Shift.Event.EventTypeId))
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
                .OrderBy(s => s.BeginTime);

            IWorkbook workbook = new XSSFWorkbook();
            var rolesTableSheet = workbook.CreateSheet("Сводка");
            int rowNum = 0;

            var titleRow = rolesTableSheet.CreateRow(rowNum++);

            int columnNum = 0;
            titleRow.CreateCell(columnNum++).SetCellValue("Фамилия");
            titleRow.CreateCell(columnNum++).SetCellValue("Имя");

            foreach (var shift in events)
            {
                titleRow.CreateCell(columnNum++).SetDateValue(shift.BeginTime);
            }

            foreach (var user in allrecords
                .Select(r => r.User)
                .Distinct()
                .OrderBy(u => u.LastName))
            {
                columnNum = 0;
                var personRow = rolesTableSheet.CreateRow(rowNum++);
                personRow.CreateCell(columnNum++).SetCellValue(user.LastName);
                personRow.CreateCell(columnNum++).SetCellValue(user.FirstName);
                foreach (var shift in events)
                {
                    var puers = shift.Places.SelectMany(p => p.PlaceUserEventRoles).Where(puer => puer.UserId == user.Id).ToList();
                    if (puers.Count == 0)
                        personRow.CreateCell(columnNum++).SetCellValue("-");
                    else
                    {
                        var roles = puers.Select(puer => puer.EventRole.Title).ToList();
                        var textForCell = roles.Count > 1 ? "ERROR" : roles[0].ToString();
                        personRow.CreateCell(columnNum++).SetCellValue(textForCell);
                    }
                    rolesTableSheet.AutoSizeColumn(columnNum);
                }
            }

            rolesTableSheet.CreateFreezePane(2, 1);
            rolesTableSheet.AutoSizeColumn(0);
            rolesTableSheet.AutoSizeColumn(1);


            var eventNamesSheet = workbook.CreateSheet("Названия событий");
            rowNum = 0;
            foreach (var shift in events)
            {
                columnNum = 0;
                var personRow = eventNamesSheet.CreateRow(rowNum++);
                personRow.CreateCell(columnNum++).SetDateValue(shift.BeginTime);
                personRow.CreateCell(columnNum++).SetCellValue(shift.Event.Title);

            }

            var fileName = Path.GetTempFileName();
            using (var writeStream = System.IO.File.OpenWrite(fileName))
            {
                workbook.Write(writeStream);
            }
            var memoryStream = new MemoryStream();
            using (var readStream = System.IO.File.OpenRead(fileName))
            {
                await readStream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "summary.xlsx");
        }
    }
}
