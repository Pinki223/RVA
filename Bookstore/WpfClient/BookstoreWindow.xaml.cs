using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfClient.Misc;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for BookstoreWindow.xaml
    /// </summary>
    public partial class BookstoreWindow : Window
    {

        public BookstoreWindow()
        {

            InitializeComponent();
            SetAdminTabVisibility();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SessionService.Instance.Session.BookstoreService.LogOut(SessionService.Instance.Token);
        }

        private void SetAdminTabVisibility()
        {
            bool isAdmin = SessionService.Instance.Session.BookstoreService.GetMemberInfo(SessionService.Instance.Token).IsAdmin;
            AdminTab.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
