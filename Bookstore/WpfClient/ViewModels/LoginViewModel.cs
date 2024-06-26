using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public string Username { get; set; }
        public Command<object> LoginCommand { get; set; }
        private string errorText;
        public string ErrorText
        {
            get { return errorText; }
            set
            {
                errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }
        private bool isLoggedIn = false;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public LoginViewModel()
        {
            LoginCommand = new Command<object>(Login);
            var sessionService = SessionService.Instance;
        }

        private void Login(object parameter)
        {
            PasswordBox pw = parameter as PasswordBox;

            var sessionService = SessionService.Instance;
            string token = sessionService.Session.BookstoreService.LogIn(Username, pw.Password);
            if(token != null)
            {
                sessionService.Token = token;
                BookstoreWindow bookstore = new BookstoreWindow();
                bookstore.Show();

                Application.Current.MainWindow.Close();
                Application.Current.MainWindow = bookstore;

                ClientLogger.Log($"Successfull.", Common.Log.LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(token).Username);
            }
            else
            {
                ErrorText = "Incorrect credentials or already logged in!";
            }
        }
    }
}
