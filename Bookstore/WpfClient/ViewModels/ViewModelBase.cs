using Common.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected static Logger ClientLogger;

        public ViewModelBase()
        {
            if (ClientLogger == null)
            {
                ClientLogger = new Logger("LogData.txt");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
