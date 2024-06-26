using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WpfClient.Classes;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    public class NewAuthorViewModel : ViewModelBase
    {
        private string firstName;
        private string lastName;
        private string shortDesc;
        private string errorMessage;

        #region Properties
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public string ShortDesc
        {
            get { return shortDesc; }
            set
            {
                shortDesc = value;
                OnPropertyChanged("ShortDesc");
            }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }
        #endregion

        public Command<Window> NewAuthorCommand { get; set; }



        public NewAuthorViewModel()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            ShortDesc = string.Empty;
            ErrorMessage = string.Empty;

            NewAuthorCommand = new Command<Window>(NewAuthor);
        }

        private void NewAuthor(Window window)
        {
            if (!ValidateAuthor())
                return;

            window.DialogResult = true;
            window.Close();
        }

        private bool ValidateAuthor()
        {
            if (FirstName == string.Empty)
            {
                ErrorMessage = "First name cannot be empty.";
                return false;
            }

            if (LastName == string.Empty)
            {
                ErrorMessage = "Last name cannot be empty.";
                return false;
            }

            if (ShortDesc == string.Empty)
            {
                ErrorMessage = "Description cannot be empty.";
                return false;
            }

            ErrorMessage = string.Empty;

            return true;
        }
    }
}
