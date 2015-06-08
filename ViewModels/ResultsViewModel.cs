using System;
using System.Collections.Generic;
using CandidateAssessment.Data;
using ReactiveUI;

namespace CandidateAssessment.ViewModels
{
    public class ResultsViewModel : ReactiveObject
    {
        private readonly ReactiveList<ImportResult> _results;
        private readonly IReactiveDerivedList<ImportedItemViewModel> _importedItems;
        private readonly IReactiveDerivedList<Tuple<int, string>> _failedItems; 

        public ResultsViewModel(IObservable<IEnumerable<ImportResult>> resultsAvailableObservable)
        {
            if (resultsAvailableObservable == null) throw new ArgumentNullException("resultsAvailableObservable");

            _results = new ReactiveList<ImportResult>();

            resultsAvailableObservable.Subscribe(r =>
            {
                using (_results.SuppressChangeNotifications())
                {
                    _results.Clear();
                    _results.AddRange(r);
                }
            });

            _importedItems = _results.CreateDerivedCollection(r => new ImportedItemViewModel(r.ItemOrValidationError.Item, r.Row),
                r => r.ItemOrValidationError.Item != null);

            _failedItems = _results.CreateDerivedCollection(r => new Tuple<int, string>(r.Row, r.ItemOrValidationError.Error),
                r => r.ItemOrValidationError.Error != null);
        }

        public IEnumerable<ImportedItemViewModel> ImportedItems
        {
            get { return _importedItems;}
        }

        public IEnumerable<Tuple<int, string>> FailedItems
        {
            get { return _failedItems;}
        }
    }
}