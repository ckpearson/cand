using System;
using System.Collections.Generic;

namespace CandidateAssessment.Data
{
    public struct ImportedData
    {
        private readonly Dictionary<string, object> _data;
        private readonly int _row;

        public ImportedData(Dictionary<string, object> data, int row)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (row < 1) throw new ArgumentOutOfRangeException("row");

            _data = data;
            _row = row;
        }

        public Dictionary<string, object> Data
        {
            get { return _data; }
        }

        public int Row
        {
            get { return _row; }
        }
    }
}