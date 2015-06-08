using System;

namespace CandidateAssessment.Data
{
    public class ImportResult
    {
        private readonly DataItemOrValidationError _itemOrValidationError;
        private readonly int _row;

        public ImportResult(DataItemOrValidationError itemOrValidationError, int row)
        {
            if (itemOrValidationError == null) throw new ArgumentNullException("itemOrValidationError");
            if (row < 1) throw new ArgumentOutOfRangeException("row");

            _itemOrValidationError = itemOrValidationError;
            _row = row;
        }

        public DataItemOrValidationError ItemOrValidationError
        {
            get { return _itemOrValidationError; }
        }

        public int Row
        {
            get { return _row; }
        }
    }
}