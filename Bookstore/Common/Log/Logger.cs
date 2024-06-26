using Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Log
{
    public class Logger
    {
        private string path;
        public event Action OnMessageLogged;

        public Logger(string filename)
        {
            path = filename;
        }

        public void Log(string message, LogLevel logLevel, string username)
        {
            string text = $"{DateTime.Now} [{logLevel.ToString().ToUpper()}]: {message} ({username})" + Environment.NewLine;

            File.AppendAllText(path, text);

            if (OnMessageLogged != null)
                OnMessageLogged();
        }
    }
}
