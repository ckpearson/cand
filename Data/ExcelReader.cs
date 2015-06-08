using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;

namespace CandidateAssessment.Data
{
    public static class ExcelReader
    {
        public static XLWorkbook OpenWorkbook(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            return new XLWorkbook(path);
        }

        public static IEnumerable<string> GetColumns(XLWorkbook workbook, string sheet, int expecting)
        {
            if (workbook == null) throw new ArgumentNullException("workbook");
            if (string.IsNullOrEmpty(sheet)) throw new ArgumentNullException("sheet");
            if (expecting < 1) throw new ArgumentOutOfRangeException("expecting");

            var bookSheet = workbook.Worksheet(sheet);
            return Enumerable.Range(1, expecting).Select(x => (string) bookSheet.Cell(1, x).Value).Where(v => !string.IsNullOrEmpty(v));
        }

        public static IEnumerable<ImportedData> ReadColumnarData(
            XLWorkbook workbook,
            string sheet,
            int numCols)
        {
            if (workbook == null) throw new ArgumentNullException("workbook");
            if (string.IsNullOrEmpty(sheet)) throw new ArgumentNullException("sheet");
            if (numCols < 1) throw new ArgumentOutOfRangeException("numCols");

            var bookSheet = workbook.Worksheet(sheet);
            var lastRow = bookSheet.LastRowUsed().RowNumber();
            var presentedHeaders = GetColumns(workbook, sheet, numCols).ToList();
            for (var row = 2; row <= lastRow; row++)
            {
                var dict = new Dictionary<string, object>();
                var runsTo = bookSheet.Row(row).LastCellUsed().WorksheetColumn().ColumnNumber();
                for (var col = 1; col <= runsTo; col++)
                {
                    dict.Add(presentedHeaders[col - 1], bookSheet.Cell(row, col).Value);
                }
                yield return new ImportedData(dict, row);
            }
        }
    }
}