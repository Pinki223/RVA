using Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using WpfClient.ViewModels;

namespace WpfClient.Properties
{
    /// <summary>
    /// Interaction logic for NewBookWindow.xaml
    /// </summary>
    public partial class NewBookWindow : Window
    {
        public NewBookWindow(List<Author> authors, Book book = null)
        {
            InitializeComponent();

            ObservableCollection<Author> authorCollection = new ObservableCollection<Author>(authors);
            ICollectionView authorView = CollectionViewSource.GetDefaultView(authorCollection);
            if (book != null)
            {
                DataContext = new NewBookViewModel(authorView, book);
            }
            else
            {
                DataContext = new NewBookViewModel(authorView);
            }
        }
    }
}
