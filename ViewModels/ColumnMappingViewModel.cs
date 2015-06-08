using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ReactiveUI;

namespace CandidateAssessment.ViewModels
{
    public class ColumnMappingViewModel : ReactiveObject
    {
        private string _sourceColumn;
        private readonly IEnumerable<string> _targetColumns;
        private string _chosenTargetColumn;

        public ColumnMappingViewModel(string sourceColumn, IEnumerable<string> targetColumns,
            string chosenTargetColumn)
        {
            if (string.IsNullOrEmpty(sourceColumn)) throw new ArgumentNullException("sourceColumn");
            if (targetColumns == null) throw new ArgumentNullException("targetColumns");
            if (string.IsNullOrEmpty(chosenTargetColumn)) throw new ArgumentNullException("chosenTargetColumn");

            _sourceColumn = sourceColumn;
            _targetColumns = targetColumns;
            _chosenTargetColumn = chosenTargetColumn;
        }

        public string SourceColumn
        {
            get { return _sourceColumn; }
            set { this.RaiseAndSetIfChanged(ref _sourceColumn, value); }
        }

        public IEnumerable<string> TargetColumns
        {
            get { return _targetColumns; }
        }

        public string ChosenTargetColumn
        {
            get { return _chosenTargetColumn; }
            set { this.RaiseAndSetIfChanged(ref _chosenTargetColumn, value); }
        }
    }
}