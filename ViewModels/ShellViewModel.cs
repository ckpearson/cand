using System;
using System.Reactive.Linq;
using ReactiveUI;

namespace CandidateAssessment.ViewModels
{
    public class ShellViewModel : ReactiveObject
    {
        private readonly ConnectionViewModel _connectionViewModel;
        private readonly ImportDetailsViewModel _importDetails;
        private readonly ImportViewModel _importViewModel;
        private readonly ResultsViewModel _resultsViewModel;
        private AppStage _currentStage;

        public ShellViewModel()
        {
            _connectionViewModel = new ConnectionViewModel();
            _importDetails = new ImportDetailsViewModel(_connectionViewModel.Connected);
            _importViewModel = new ImportViewModel(_connectionViewModel.Connected, _importDetails.ImportRequest);
            _resultsViewModel = new ResultsViewModel(_importViewModel.ResultsAvailable);

            this.WhenAnyObservable(v => v.Connection.Connected)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(conn => CurrentStage = AppStage.PreImport);

            this.WhenAnyObservable(v => v.ImportDetails.ImportRequest)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => CurrentStage = AppStage.ImportProgress);

            this.WhenAnyObservable(v => v.Import.ResultsAvailable)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => CurrentStage = AppStage.ImportResult);
        }

        public ConnectionViewModel Connection
        {
            get { return _connectionViewModel; }
        }

        public ImportDetailsViewModel ImportDetails
        {
            get { return _importDetails; }
        }

        public ImportViewModel Import
        {
            get { return _importViewModel; }
        }

        public ResultsViewModel Results
        {
            get { return _resultsViewModel;}
        }

        public AppStage CurrentStage
        {
            get { return _currentStage;}
            private set { this.RaiseAndSetIfChanged(ref _currentStage, value); }
        }
    }
}