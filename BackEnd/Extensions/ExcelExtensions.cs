using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Extensions
{
    public static class ExcelExtensions
    {
        public static ICell SetDateValue(this ICell cell, DateTime date)
        {
            cell.SetCellValue(date.ToString("dd.MM"));
            return cell;
        }
    }
}
