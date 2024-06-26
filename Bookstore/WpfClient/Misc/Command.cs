using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfClient.Misc
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged = delegate { };

        private Action callback;
        private Func<bool> canExecuteMethod;



        public Command(Action callbackFunc, Func<bool> canExecute = null)
        {
            callback = callbackFunc;
            canExecuteMethod = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteMethod != null ? canExecuteMethod() : callback != null;
        }

        public void Execute(object parameter)
        {
            if (callback != null)
                callback();
        }
    }

    public class Command<T> : ICommand
    {
        public event EventHandler CanExecuteChanged = delegate { };

        private Action<T> callback;
        private Func<bool> canExecuteMethod;



        public Command(Action<T> callbackFunc, Func<bool> canExecute = null)
        {
            callback = callbackFunc;
            canExecuteMethod = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteMethod != null ? canExecuteMethod() : callback != null;
        }

        public void Execute(object parameter)
        {
            if (callback != null)
                callback((T)parameter);
        }
    }
}
