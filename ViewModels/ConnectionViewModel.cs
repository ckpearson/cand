using System;
using System.Data.Common;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using CandidateAssessment.Data;
using ReactiveUI;

namespace CandidateAssessment.ViewModels
{
    public class ConnectionViewModel : ReactiveObject
    {
        private bool _isConnecting;
        private string _server;
        private string _database;
        private readonly ReactiveCommand<object> _connectCommand;
        private readonly ReplaySubject<DbConnection> _connectionSubject;

        public ConnectionViewModel()
        {
            _connectCommand = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(v => v.Server, v => v.Database)
                .Select(t => !string.IsNullOrEmpty(t.Item1) && !string.IsNullOrEmpty(t.Item2)),
                _ => ConnectImpl());
            _connectionSubject = new ReplaySubject<DbConnection>();
        }

        public IObservable<DbConnection> Connected
        {
            get { return _connectionSubject; }
        }

        private IObservable<object> ConnectImpl()
        {
            IsConnecting = true;
            return Observable.FromAsync(_ => DatabaseAccess.GetConnection(Server, Database))
                .Do(conn => { _connectionSubject.OnNext(conn); })
                .Catch<DbConnection, Exception>(ex =>
                {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return Observable.Empty<DbConnection>();
                })
                .Finally(() => IsConnecting = false);
        }

        public ReactiveCommand<object> Connect
        {
            get { return _connectCommand; }
        }

        public string Server
        {
            get { return _server; }
            set { this.RaiseAndSetIfChanged(ref _server, value); }
        }

        public string Database
        {
            get { return _database; }
            set { this.RaiseAndSetIfChanged(ref _database, value); }
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            private set { this.RaiseAndSetIfChanged(ref _isConnecting, value); }
        }
    }
}