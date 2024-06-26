using Common.Model;
using Common.Log;
using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Data;
using WpfClient.Misc;
using System.Windows.Controls;
using WpfClient.Properties;
using System.Windows.Input;
using WpfClient.Misc;
using WpfClient.Classes;

namespace WpfClient.ViewModels
{
    internal class BookViewModel : ViewModelBase
    {
        public ICollectionView BookList { get; set; }
        public List<Author> AuthorList { get; set; }
        public string BookNameTextBox { get; set; }
        public string AuthorTextBox { get; set; }

        // Commands
        public Command NewAuthorCommand { get; set; }
        public Command NewBookCommand { get; set; }
        public Command EditBookCommand { get; set; }
        public Command DuplicateCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public Command LeaseCommand { get; set; }
        public Command SearchCommand { get; set; }
        public Command ReturnCommand { get; set; }

        // Undo/redo
        public Command UndoCommand { get; set; }
        public Command RedoCommand { get; set; }
        private Classes.ActionHistory history;

        private Book selectedBook;
        private List<Book> books;
        private bool isAdmin = false;

        public BookViewModel()
        {
            history = new Classes.ActionHistory();

            NewAuthorCommand = new Command(NewAuthor, IsAdmin);
            NewBookCommand = new Command(NewBook, IsAdmin);
            EditBookCommand = new Command(EditBook, IsAdmin);
            DuplicateCommand = new Command(DuplicateBook, CanDuplicate);
            DeleteCommand = new Command(DeleteBook, IsAdmin);
            RefreshCommand = new Command(RefreshList);
            LeaseCommand = new Command(LeaseBook, CanLease);
            ReturnCommand = new Command(ReturnBook, CanReturn);
            SearchCommand = new Command(FilterBooks);
            UndoCommand = new Command(Undo, history.CanUndo);
            RedoCommand = new Command(Redo, history.CanRedo);

            RefreshList();
        }

        public Book SelectedBook
        {
            get { return selectedBook; }
            set
            {
                selectedBook = value;
                DuplicateCommand.RaiseCanExecuteChanged();
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedBook");
            }
        }

        public void NewAuthor()
        {
            var win = new NewAuthorWindow();
            NewAuthorViewModel vm = (NewAuthorViewModel)win.DataContext;
            bool? result = win.ShowDialog();
            if (result == true)
            {
                var sessionService = SessionService.Instance;
                if (sessionService.Session.BookstoreService.CreateAuthor(vm.FirstName, vm.LastName, vm.ShortDesc, sessionService.Token) != null)
                {
                    ClientLogger.Log($"Author {vm.FirstName} {vm.LastName} successfully created.", LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                }
                else
                {
                    ClientLogger.Log($"Author {vm.FirstName} {vm.LastName} could not be created.", LogLevel.ERROR, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                }
            }
        }

        private void NewBook()
        {
            var win = new NewBookWindow(AuthorList);
            NewBookViewModel vm = (NewBookViewModel)win.DataContext;

            bool? result = win.ShowDialog();
            if (result == true)
            {
                var sessionService = SessionService.Instance;
                var newBook = new Book
                {
                    Title = vm.BookName,
                    PublishYear = int.Parse(vm.PublicationYear),
                    AuthorId = vm.SelectedAuthor.AuthorId
                };

                Action undo = () =>
                {
                    sessionService.Session.BookstoreService.DeleteBook(books.Last().BookId, sessionService.Token);
                    RefreshList();
                };

                Action redo = () =>
                {
                    if (sessionService.Session.BookstoreService.CreateBook(newBook.Title, newBook.PublishYear, newBook.AuthorId, sessionService.Token) > 0)
                    {
                        ClientLogger.Log($"Book {vm.BookName} successfully created.", LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                    }
                    else
                    {
                        ClientLogger.Log($"Book {vm.BookName} could not be created.", LogLevel.ERROR, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                    }
                    RefreshList();
                };

                history.AddAndExecute(new RevertableCommand(redo, undo));
                UndoCommand.RaiseCanExecuteChanged();
                RedoCommand.RaiseCanExecuteChanged();
            }

            RefreshList();
        }

        private void EditBook()
        {
            var sessionService = SessionService.Instance;

            var originalBook = new Book
            {
                BookId = selectedBook.BookId,
                Title = selectedBook.Title,
                PublishYear = selectedBook.PublishYear,
                Author = new Author
                {
                    AuthorId = selectedBook.Author.AuthorId,
                    FirstName = selectedBook.Author.FirstName,
                    LastName = selectedBook.Author.LastName
                },
                AuthorId = selectedBook.AuthorId,
                LastModified = selectedBook.LastModified
            };

            var win = new NewBookWindow(AuthorList, originalBook);
            NewBookViewModel vm = (NewBookViewModel)win.DataContext;

            bool? result = win.ShowDialog();
            if (result == true)
            {
                if (sessionService.Session.BookstoreService.EditBook(selectedBook.BookId, vm.BookName, int.Parse(vm.PublicationYear), vm.SelectedAuthor.AuthorId, sessionService.Token))
                {
                    ClientLogger.Log($"Book {vm.BookName} successfully edited.", LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                }
                else
                {
                    ClientLogger.Log($"Book {vm.BookName} could not be edited.", LogLevel.ERROR, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                }

                var editedBook = new Book
                {
                    BookId = selectedBook.BookId,
                    Title = vm.BookName,
                    PublishYear = int.Parse(vm.PublicationYear),
                    Author = vm.SelectedAuthor
                };

                Action undo = () =>
                {
                    sessionService.Session.BookstoreService.EditBook(
                        originalBook.BookId, originalBook.Title, originalBook.PublishYear, originalBook.Author.AuthorId, sessionService.Token);
                    selectedBook.Title = originalBook.Title;
                    selectedBook.PublishYear = originalBook.PublishYear;
                    selectedBook.Author = originalBook.Author;
                    OnPropertyChanged(nameof(SelectedBook));
                    RefreshList();
                };

                Action redo = () =>
                {
                    sessionService.Session.BookstoreService.EditBook(
                        editedBook.BookId, editedBook.Title, editedBook.PublishYear, editedBook.Author.AuthorId, sessionService.Token);
                    selectedBook.Title = editedBook.Title;
                    selectedBook.PublishYear = editedBook.PublishYear;
                    selectedBook.Author = editedBook.Author;
                    OnPropertyChanged(nameof(SelectedBook));
                    RefreshList();
                };

                redo();

                history.AddAndExecute(new RevertableCommand(redo, undo));
                UndoCommand.RaiseCanExecuteChanged();
                RedoCommand.RaiseCanExecuteChanged();
            }

            RefreshList();
        }

        private void DuplicateBook()
        {
            var sessionService = SessionService.Instance;
            var originalBook = new Book
            {
                Title = selectedBook.Title,
                PublishYear = selectedBook.PublishYear,
                AuthorId = selectedBook.Author.AuthorId
            };

            int deleteId = selectedBook.BookId;

            Action undo = () =>
            {
                sessionService.Session.BookstoreService.DeleteBook(deleteId, sessionService.Token);
                RefreshList(); ;
            };

            Action redo = () =>
            {
                deleteId = sessionService.Session.BookstoreService.CloneBook(originalBook, sessionService.Token);
                ClientLogger.Log($"Book {selectedBook.Title} duplicated.", LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);

                RefreshList();
            };

            history.AddAndExecute(new RevertableCommand(redo, undo));
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private void DeleteBook()
        {
            var sessionService = SessionService.Instance;

            var originalBook = new Book
            {
                Title = selectedBook.Title,
                PublishYear = selectedBook.PublishYear,
                AuthorId = selectedBook.Author.AuthorId
            };

            int bookId = selectedBook.BookId;

            Action undo = () =>
            {
                bookId = sessionService.Session.BookstoreService.CloneBook(originalBook, sessionService.Token);
                RefreshList();
            };

            Action redo = () =>
            {
                RefreshList();
                if (sessionService.Session.BookstoreService.DeleteBook(bookId, sessionService.Token))
                {
                    ClientLogger.Log($"Book {selectedBook.Title} deleted successfully.", LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                }
                else
                {
                    ClientLogger.Log($"Book {selectedBook.Title} could not be deleted.", LogLevel.ERROR, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
                }
                RefreshList();
            };

            if (sessionService.Session.BookstoreService.WasEdited(selectedBook.BookId, selectedBook.LastModified, sessionService.Token))
            {
                var messageBoxResult = MessageBox.Show("This book has been modified by another user. Do you still want to delete it?", "Conflict", MessageBoxButton.YesNo);
                if(messageBoxResult == MessageBoxResult.No)
                {
                    RefreshList();
                    return;
                }
            }

            history.AddAndExecute(new RevertableCommand(redo, undo));
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private bool CanDuplicate()
        {
            return SelectedBook != null && IsAdmin();
        }

        private void RefreshList()
        {
            using (var c = new WaitCursor())
            {
                var book = SelectedBook;
                var session = SessionService.Instance.Session;
                Dictionary<int, Book> booksDictionary = session.BookstoreService.GetBooks();
                books = booksDictionary.Values.ToList();

                AuthorList = session.BookstoreService.GetAuthors();

                CollectionViewSource itemSourceList = new CollectionViewSource() { Source = books };
                BookList = itemSourceList.View;

                bool AdminStatus = SessionService.Instance.Session.BookstoreService.GetMemberInfo(SessionService.Instance.Token).IsAdmin;
                isAdmin = AdminStatus;
                NewAuthorCommand.RaiseCanExecuteChanged();
                NewBookCommand.RaiseCanExecuteChanged();
                EditBookCommand.RaiseCanExecuteChanged();
                DuplicateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();

                OnPropertyChanged(nameof(BookList));
            }
        }


        private void LeaseBook()
        {
            var sessionService = SessionService.Instance;
            var originalBook = new Book
            {
                BookId = selectedBook.BookId,
                Title = selectedBook.Title,
                PublishYear = selectedBook.PublishYear,
                Username = selectedBook.Username
            };

            int bookId = originalBook.BookId;

            Action undo = () =>
            {
                sessionService.Session.BookstoreService.ReturnBook(bookId, sessionService.Token);
                selectedBook.Username = null;
                OnPropertyChanged(nameof(SelectedBook));
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                RefreshList();
            };

            Action redo = () =>
            {
                sessionService.Session.BookstoreService.LeaseBook(bookId, sessionService.Token);
                selectedBook.Username = sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username;
                OnPropertyChanged(nameof(SelectedBook));
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                RefreshList();
            };


            history.AddAndExecute(new RevertableCommand(redo, undo));
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();

            ClientLogger.Log($"{sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username} leased book {selectedBook.Title}", LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);
        }

        private bool CanLease()
        {
            return selectedBook != null && selectedBook.Username == null;
        }

        private bool IsAdmin()
        {
            return isAdmin;
        }

        private void ReturnBook()
        {
            var sessionService = SessionService.Instance;
            var originalBook = new Book
            {
                BookId = selectedBook.BookId,
                Title = selectedBook.Title,
                PublishYear = selectedBook.PublishYear,
                Username = selectedBook.Username
            };

            int bookId = originalBook.BookId;

            Action undo = () =>
            {
                sessionService.Session.BookstoreService.LeaseBook(bookId, sessionService.Token);
                selectedBook.Username = sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username;
                OnPropertyChanged(nameof(SelectedBook));
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                RefreshList();
            };

            Action redo = () =>
            {
                sessionService.Session.BookstoreService.ReturnBook(bookId, sessionService.Token);
                selectedBook.Username = null;
                OnPropertyChanged(nameof(SelectedBook));
                LeaseCommand.RaiseCanExecuteChanged();
                ReturnCommand.RaiseCanExecuteChanged();
                RefreshList();
            };


            history.AddAndExecute(new RevertableCommand(redo, undo));
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();

            ClientLogger.Log($"{sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username} returned book {selectedBook.Title}", LogLevel.INFO, sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username);

        }

        private bool CanReturn()
        {
            var sessionService = SessionService.Instance;
            var bookUsername = sessionService.Session.BookstoreService.GetMemberInfo(sessionService.Token).Username;
            return selectedBook != null && selectedBook.Username != null && selectedBook.Username == bookUsername;
        }

        private void FilterBooks()
        {
            BookList.Filter = new Predicate<object>(b =>
                ((Book)b).Title.Contains(BookNameTextBox == null ? "" : BookNameTextBox) &&
                ((Book)b).Author.FirstName.Contains(AuthorTextBox == null ? "" : AuthorTextBox)
            );
        }

        private void Undo()
        {
            history.Undo();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }

        private void Redo()
        {
            history.Redo();
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
        }
    }
}
