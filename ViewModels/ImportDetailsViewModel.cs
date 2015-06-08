using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CandidateAssessment.Data;
using ClosedXML.Excel;
using Microsoft.Win32;
using ReactiveUI;

namespace CandidateAssessment.ViewModels
{
    public class ImportDetailsViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<XLWorkbook> _currentWorkbook;
        private readonly ObservableAsPropertyHelper<IEnumerable<string>> _availableSheets;
        private readonly ReactiveList<string> _availableTables; 
        private readonly ObservableAsPropertyHelper<string> _fileNameDisplay;
        private readonly ObservableAsPropertyHelper<bool> _hasWorkbook; 
        private readonly ReactiveCommand<object> _chooseFile;
        private readonly ReactiveCommand<object> _import;
        private readonly Subject<ImportDetails> _importRequest;
        private readonly ObservableAsPropertyHelper<ColumnMappingsViewModel> _columnMappings; 
        private string _chosenSheet;
        private string _chosenFilePath;
        private string _chosenDatabaseTable;

        public ImportDetailsViewModel(IObservable<DbConnection> connectionObservable)
        {
            if (connectionObservable == null) throw new ArgumentNullException("connectionObservable");

            _availableTables = new ReactiveList<string>();

            // Only get the tables that schematically support importing
            connectionObservable.SelectMany(c => Observable.StartAsync(_ => DatabaseAccess.GetTableNames(c))
                .SelectMany(
                    tns =>
                        tns.Select(
                            tn =>
                                Observable.Defer(
                                    () => Observable.StartAsync(() => DatabaseAccess.GetColumnNames(c, tn)).Select(cols => new {tbl = tn, cols})))
                            .Merge(5)
                            .Where(dets => DtoDataItem.ColumnNames.All(dets.cols.Contains))
                            .Select(dets => dets.tbl)))
                .Subscribe(_availableTables.Add);

            _chooseFile = ReactiveCommand.Create();
            _chooseFile.Subscribe(_ => ChooseFileImp());
            _importRequest = new Subject<ImportDetails>();

            this.WhenAnyValue(v => v.ChosenFilePath)
                .Select(Path.GetFileName)
                .ToProperty(this, v => v.ChosenFileDisplayName, out _fileNameDisplay);

            this.WhenAnyValue(v => v.ChosenFilePath).Where(v => !string.IsNullOrEmpty(v))
                .Select(ExcelReader.OpenWorkbook)
                .ToProperty(this, v => v.CurrentWorkbook, out _currentWorkbook);

            this.WhenAnyValue(v => v.CurrentWorkbook)
                .Select(v => v != null)
                .ToProperty(this, v => v.HasWorkbook, out _hasWorkbook);

            this.WhenAnyValue(v => v.CurrentWorkbook)
                .Select(w => w == null ? Enumerable.Empty<string>() : w.Worksheets.Select(s => s.Name))
                .ToProperty(this, v => v.AvailableSheets, out _availableSheets);

            // Build column mappings based on chosen details
            connectionObservable.CombineLatest(this.WhenAnyValue(v => v.CurrentWorkbook, v => v.ChosenSheet, v => v.ChosenDatabaseTable)
                .Select(t => new {Wbk = t.Item1, Sheet = t.Item2, Table = t.Item3}),
                (conn, items) => new {conn, items.Wbk, items.Sheet, items.Table})
                .Where(d => d.Wbk != null && !string.IsNullOrEmpty(d.Sheet) && !string.IsNullOrEmpty(d.Table))
                .SelectMany(d =>
                    Observable.Return(DtoDataItem.ColumnNames)
                        .CombineLatest(Observable.StartAsync(() => Task.Run(() => ExcelReader.GetColumns(d.Wbk, d.Sheet, 4))),
                            (dbcols, excelcols) =>
                            {
                                var dcols = dbcols.ToList();
                                var ecols = excelcols.ToList();
                                return
                                    new ColumnMappingsViewModel(dcols.Select((db, x) => new ColumnMappingViewModel(ecols.ElementAtOrDefault(x),
                                        dcols, dcols.ElementAtOrDefault(x))));
                            }))
                .ToProperty(this, v => v.ColumnMappings, out _columnMappings);

            _import = ReactiveCommand.Create(this.WhenAnyValue(v => v.ChosenFilePath, v => v.ChosenDatabaseTable, v => v.ChosenSheet)
                .Select(t => !string.IsNullOrEmpty(t.Item1) && !string.IsNullOrEmpty(t.Item2) && !string.IsNullOrEmpty(t.Item3)));

            _import.Subscribe(
                _ => _importRequest.OnNext(new ImportDetails(CurrentWorkbook, ChosenSheet, ChosenDatabaseTable, ColumnMappings.MappingDictionary())));
        }

        public XLWorkbook CurrentWorkbook
        {
            get { return _currentWorkbook.Value; }
        }

        public bool HasWorkbook
        {
            get { return _hasWorkbook.Value; }
        }

        public string ChosenFileDisplayName
        {
            get { return _fileNameDisplay.Value; }
        }

        public ColumnMappingsViewModel ColumnMappings
        {
            get { return _columnMappings.Value; }
        }

        public ReactiveCommand<object> ChooseFile
        {
            get { return _chooseFile; }
        }

        public ReactiveCommand<object> Import
        {
            get { return _import; }
        }

        public IObservable<ImportDetails> ImportRequest
        {
            get { return _importRequest;}
        }

        private void ChooseFileImp()
        {
            var ofd = new OpenFileDialog()
            {
                Title = "Choose Import File",
                Filter = "Excel Workbooks|*.xlsx",
                Multiselect = false,
            };
            var res = ofd.ShowDialog();
            if (res == null || !res.Value) return;
            ChosenFilePath = ofd.FileName;
        }

        public string ChosenFilePath
        {
            get { return _chosenFilePath; }
            private set { this.RaiseAndSetIfChanged(ref _chosenFilePath, value); }
        }

        public string ChosenDatabaseTable
        {
            get { return _chosenDatabaseTable;}
            set { this.RaiseAndSetIfChanged(ref _chosenDatabaseTable, value); }
        }

        public string ChosenSheet
        {
            get { return _chosenSheet; }
            set { this.RaiseAndSetIfChanged(ref _chosenSheet, value); }
        }

        public IEnumerable<string> AvailableTables
        {
            get { return _availableTables; }
        }

        public IEnumerable<string> AvailableSheets
        {
            get { return _availableSheets.Value; }
        }
    }
}