using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    internal class AccountViewModel : ViewModelBase
    {
        public string FirstNameTextBox { get; set; }
        public string LastNameTextBox { get; set; }

        public Command SaveCommand { get; set; }



        public AccountViewModel()
        {
            var sessionService = SessionService.Instance;
            string token = sessionService.Token;
            Session session = new Session();
            Member user = session.BookstoreService.GetMemberInfo(token);
            FirstNameTextBox = user.FirstName;
            LastNameTextBox = user.LastName;

            SaveCommand = new Command(SaveAccountChanges);
        }

        private void SaveAccountChanges()
        {
            var sessionService = SessionService.Instance;
            string token = sessionService.Token;
            Session session = new Session();
            if(session.BookstoreService.EditMemberInfo(FirstNameTextBox, LastNameTextBox, token))
            {
                ClientLogger.Log($"Account {sessionService.Session.BookstoreService.GetMemberInfo(token).Username} successfully edited.", Common.Log.LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(token).Username);
            }
            else
            {
                ClientLogger.Log($"Account {sessionService.Session.BookstoreService.GetMemberInfo(token).Username} couldn't be edited.", Common.Log.LogLevel.ERROR, sessionService.Session.BookstoreService.GetMemberInfo(token).Username);
            }
        }
    }
}
