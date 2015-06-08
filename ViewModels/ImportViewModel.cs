using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CandidateAssessment.Data;
using ReactiveUI;

namespace CandidateAssessment.ViewModels
{
    public class ImportViewModel : ReactiveObject
    {
        private string _message;
        private readonly Subject<IEnumerable<ImportResult>> _resultsAvailable;

        public ImportViewModel(IObservable<DbConnection> connectionObservable, IObservable<ImportDetails> importDetailsObservable)
        {
            if (connectionObservable == null) throw new ArgumentNullException("connectionObservable");
            if (importDetailsObservable == null) throw new ArgumentNullException("importDetailsObservable");

            _resultsAvailable = new Subject<IEnumerable<ImportResult>>();

            connectionObservable.CombineLatest(importDetailsObservable, (c, d) => new {Connection = c, Details = d})
                .Do(_ =>
                {
                    Message = "Reading Data";
                })
                .SelectMany(
                    d => Observable.Start(() => ExcelReader.ReadColumnarData(d.Details.Workbook, d.Details.Sheet, d.Details.ColumnMappings.Count)
                        .Select(dat => new ImportedData(SubstituteKeys(dat.Data, d.Details.ColumnMappings), dat.Row))
                        .Select(dat => new ImportResult(DtoDataItem.FromData(dat.Data), dat.Row))
                        .ToList()).Select(dt => new {Data = dt, Details = d}))
                .Do(_ => Message = "Uploading Data")
                .SelectMany(d => Observable.StartAsync(() => DatabaseAccess.InsertInto(d.Details.Connection, d.Details.Details.Table,
                    d.Data.Where(i => i.ItemOrValidationError.Item != null).Select(i => i.ItemOrValidationError.Item),
                    DtoDataItem.ColumnNames.ToArray())).Select(_ => d))
                .Select(d => d.Data)
                .Do(_ => { Message = "Preparing Results"; })
                .Subscribe(_resultsAvailable.OnNext);
        }

        public IObservable<IEnumerable<ImportResult>> ResultsAvailable
        {
            get { return _resultsAvailable;}
        }

        public string Message
        {
            get { return _message; }
            private set { this.RaiseAndSetIfChanged(ref _message, value); }
        }

        private static Dictionary<string, object> SubstituteKeys(Dictionary<string, object> dict, Dictionary<string, string> substitutions)
        {
            return dict.ToDictionary(k => substitutions[k.Key], k => k.Value);
        }
    }
}