using System;
using CandidateAssessment.Data;

namespace CandidateAssessment.ViewModels
{
    public class ImportedItemViewModel
    {
        private readonly DtoDataItem _item;
        private readonly int _row;

        public ImportedItemViewModel(DtoDataItem item, int row)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (row < 1) throw new ArgumentOutOfRangeException("row");

            _item = item;
            _row = row;
        }

        public DtoDataItem Item
        {
            get { return _item; }
        }

        public int Row
        {
            get { return _row; }
        }
    }
}