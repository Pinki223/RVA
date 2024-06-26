using Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WpfClient.Classes;
using WpfClient.Misc;

namespace WpfClient.ViewModels
{
    public class NewBookViewModel : ViewModelBase
    {
        private string bookName;
        private string publicationYear;
        private string errorMessage;
        private ICollectionView authorList;
        private Author selectedAuthor;
        private DateTime lastModified;
        private int bookId;

        #region Properties
        public string BookName
        {
            get { return bookName; }
            set
            {
                bookName = value;
                OnPropertyChanged("BookName");
            }
        }

        public string PublicationYear
        {
            get { return publicationYear; }
            set
            {
                publicationYear = value;
                OnPropertyChanged("PublicationYear");
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

        public ICollectionView AuthorList
        {
            get { return authorList; }
            set
            {
                authorList = value;
                OnPropertyChanged("AuthorList");
            }
        }

        public Author SelectedAuthor
        {
            get { return selectedAuthor; }
            set
            {
                selectedAuthor = value;
                OnPropertyChanged("SelectedAuthor");
            }
        }
        #endregion

        public Command<Window> NewBookCommand { get; set; }

        public NewBookViewModel()
        {
            BookName = string.Empty;
            AuthorList = new CollectionView(new ObservableCollection<Author>());
            PublicationYear = string.Empty;
            lastModified = DateTime.MinValue;
            ErrorMessage = string.Empty;

            NewBookCommand = new Command<Window>(NewBook);
        }


        public NewBookViewModel(ICollectionView authors)
        {
            BookName = string.Empty;
            AuthorList = authors;
            PublicationYear = string.Empty;
            lastModified = DateTime.MinValue;
            ErrorMessage = string.Empty;

            NewBookCommand = new Command<Window>(NewBook);
        }

        public NewBookViewModel(ICollectionView authors, Book book)
        {
            if(book != null)
            {
                bookId = book.BookId;
                BookName = book.Title;
                AuthorList = authors;
                PublicationYear = book.PublishYear.ToString();
                SelectedAuthor = authors.Cast<Author>().FirstOrDefault(a => a.AuthorId == book.AuthorId);
                lastModified = book.LastModified;
                ErrorMessage = string.Empty;

                NewBookCommand = new Command<Window>(NewBook);
            }
        }

        private void NewBook(Window window)
        {
            if (!ValidateBook())
                return;

            if(lastModified != DateTime.MinValue)
            {
                var sessionService = SessionService.Instance;
                if (sessionService.Session.BookstoreService.WasEdited(bookId, lastModified, sessionService.Token))
                {
                    var messageBoxResult = MessageBox.Show("This book has been modified by another user. Do you want to overwrite the changes?", "Conflict", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        window.DialogResult = true;
                        window.Close();
                        return;
                    }
                    else
                    {
                        window.DialogResult = false;
                        window.Close();
                        return;
                    }
                }
            }
            window.DialogResult = true;
            window.Close();
        }

        private bool ValidateBook()
        {
            if (BookName == string.Empty)
            {
                ErrorMessage = "Book name cannot be empty.";
                return false;
            }

            if (SelectedAuthor == null)
            {
                ErrorMessage = "Author must be selected.";
                return false;
            }

            if (!Regex.IsMatch(PublicationYear, @"^[1-9][0-9]{3}$"))
            {
                ErrorMessage = "Publication year must be a valid year.";
                return false;
            }

            ErrorMessage = string.Empty;

            return true;
        }
    }
}
