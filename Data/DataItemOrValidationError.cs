using System;

namespace CandidateAssessment.Data
{
    public class DataItemOrValidationError
    {
        private readonly DtoDataItem _item;
        private readonly string _error;

        public DataItemOrValidationError(DtoDataItem item, string error)
        {
            if (item == null && error == null) throw new InvalidOperationException("Must supply at least one item");

            _item = item;
            _error = error;
        }

        public DtoDataItem Item
        {
            get { return _item; }
        }

        public string Error
        {
            get { return _error; }
        }
    }
}