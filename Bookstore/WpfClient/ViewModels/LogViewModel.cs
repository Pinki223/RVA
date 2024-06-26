using Common.Log;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfClient.Misc;
using System.IO;

namespace WpfClient.ViewModels
{
    class LogEntry
    {
        public LogLevel Level { get; set; }
        public string Time { get; set; }
        public string Message { get; set; }
    }


    class LogViewModel : ViewModelBase
    {
        public ObservableCollection<LogEntry> Entries { get; set; }
        Member user;

        public LogViewModel()
        {
            var sessionService = SessionService.Instance;
             user = sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token);

            Entries = new ObservableCollection<LogEntry>();

            RefreshTable();
            ClientLogger.OnMessageLogged += RefreshTable;
        }

        private void RefreshTable()
        {
            Entries.Clear();
            string[] lines = File.ReadAllLines("LogData.txt");

            foreach (string line in lines)
            {
                Match regex = Regex.Match(line, @"(?<=\().*?(?=\))");
                if (!user.Username.Equals(regex.Value))
                    continue;

                LogEntry entry = new LogEntry();
                entry.Time = line.Split('[')[0];

                regex = Regex.Match(line, @"\[[a-zA-Z]+\]");
                switch (regex.Value)
                {
                    case "[DEBUG]":
                        entry.Level = LogLevel.DEBUG;
                        break;
                    case "[INFO]":
                        entry.Level = LogLevel.INFO;
                        break;
                    case "[WARN]":
                        entry.Level = LogLevel.WARN;
                        break;
                    case "[ERROR]":
                        entry.Level = LogLevel.ERROR;
                        break;
                    case "[FATAL]":
                        entry.Level = LogLevel.FATAL;
                        break;
                }

                entry.Message = line.Split(':')[3];

                Entries.Add(entry);
            }
        }
    }
}
