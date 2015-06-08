using System;
using System.Collections.Generic;
using ClosedXML.Excel;

namespace CandidateAssessment.Data
{
    public struct ImportDetails
    {
        private readonly XLWorkbook _workbook;
        private readonly string _sheet;
        private readonly string _table;
        private readonly Dictionary<string, string> _columnMappings;

        public ImportDetails(XLWorkbook workbook, string sheet, string table, Dictionary<string, string> columnMappings)
        {
            if (workbook == null) throw new ArgumentNullException("workbook");
            if (string.IsNullOrEmpty(sheet)) throw new ArgumentNullException("sheet");
            if (columnMappings == null) throw new ArgumentNullException("columnMappings");

            _workbook = workbook;
            _sheet = sheet;
            _table = table;
            _columnMappings = columnMappings;
        }

        public string Table
        {
            get { return _table; }
        }

        public XLWorkbook Workbook
        {
            get { return _workbook; }
        }

        public Dictionary<string, string> ColumnMappings
        {
            get { return _columnMappings; }
        }

        public string Sheet
        {
            get { return _sheet; }
        }
    }
}