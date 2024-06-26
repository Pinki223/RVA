using Common;
using Common.Log;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookstoreBackend
{
    // verovatno treba izmeniti neke metode i dodati logger
    class BookstoreService : IBooks, IAuthor, IUser, IBookstoreService
    {
        private static readonly UserService userService = UserService.GetInstance();
        private readonly Logger logger;
        private static readonly object locker = new object();

        public BookstoreService()
        {
            logger = new Logger("LogData.txt");
        }

        #region BOOK
        public Dictionary<int, Book> GetBooks()
        {
            lock (locker)
            {
                using (var db = new BookstoreDbContext())
                {
                    logger.Log("Book data queried.", LogLevel.DEBUG, "");
                    return db.Books.Include("Author").ToDictionary(b => b.BookId);
                }
            }
        }

        public int CreateBook(string title, int publishmentYear, int authorId, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null || !member.IsAdmin)
                {
                    logger.Log("Failed to create book.", LogLevel.WARN, member?.Username ?? "");
                    return -1;
                }

                using (var db = new BookstoreDbContext())
                {
                    if (db.Books.Any(b => b.Title == title && b.AuthorId == authorId))
                    {
                        logger.Log("Failed to create book.", LogLevel.WARN, member.Username);
                        return -1;
                    }

                    var book = new Book { Title = title, AuthorId = authorId, PublishYear = publishmentYear, LastModified = DateTime.Now };
                    db.Books.Add(book);
                    db.SaveChanges();
                    logger.Log($"{member.Username} successfully added book.", LogLevel.INFO, member.Username);
                    return book.BookId;
                }
            }
        }

        public bool DeleteBook(int bookId, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null || !member.IsAdmin)
                {
                    logger.Log("Failed to delete book.", LogLevel.WARN, member?.Username ?? "");
                    return false;
                }

                using (var db = new BookstoreDbContext())
                {
                    var book = db.Books.Find(bookId);
                    if (book == null || book.Member != null)
                    {
                        logger.Log("Failed to delete book.", LogLevel.WARN, member.Username);
                        return false;
                    }

                    db.Books.Remove(book);
                    db.SaveChanges();
                    logger.Log($"{member.Username} successfully deleted book.", LogLevel.INFO, member.Username);
                    return true;
                }
            }
        }

        public int CloneBook(Book book, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null || !member.IsAdmin)
                {
                    logger.Log("Failed to duplicate book.", LogLevel.WARN, member?.Username ?? "");
                    return -1;
                }

                using (var db = new BookstoreDbContext())
                {
                    var clone = (Book)book.Clone();
                    db.Books.Add(clone);
                    db.SaveChanges();
                    logger.Log($"{member.Username} successfully duplicated a book.", LogLevel.INFO, member.Username);
                    return clone.BookId;
                }
            }
        }

        public bool EditBook(int bookId, string title, int publishYear, int authorId, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null || !member.IsAdmin)
                {
                    logger.Log("Failed to edit book.", LogLevel.WARN, member?.Username ?? "");
                    return false;
                }

                using (var db = new BookstoreDbContext())
                {
                    var book = db.Books.Find(bookId);
                    if (book == null)
                    {
                        logger.Log("Failed to edit book.", LogLevel.WARN, member.Username);
                        return false;
                    }

                    book.Title = title;
                    book.PublishYear = publishYear;
                    book.AuthorId = authorId;
                    book.LastModified = DateTime.Now;
                    db.SaveChanges();
                    logger.Log($"{member.Username} successfully edited a book.", LogLevel.INFO, member.Username);
                    return true;
                }
            }
        }

        public bool LeaseBook(int bookId, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null)
                {
                    logger.Log("Failed to lease a book.", LogLevel.WARN, "");
                    return false;
                }

                using (var db = new BookstoreDbContext())
                {
                    var book = db.Books.Find(bookId);
                    if (book == null)
                    {
                        logger.Log("Failed to lease a book.", LogLevel.WARN, member.Username);
                        return false;
                    }

                    book.Username = member.Username;
                    db.SaveChanges();
                    logger.Log($"{member.Username} successfully leased a book.", LogLevel.INFO, member.Username);
                    return true;
                }
            }
        }

        public bool ReturnBook(int bookId, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null)
                {
                    logger.Log("Failed to return a book.", LogLevel.WARN, "");
                    return false;
                }

                using (var db = new BookstoreDbContext())
                {
                    var book = db.Books.Find(bookId);
                    if (book == null)
                    {
                        logger.Log("Failed to return a book.", LogLevel.WARN, member.Username);
                        return false;
                    }

                    book.Username = null;
                    db.SaveChanges();
                    logger.Log($"{member.Username} successfully returned a book.", LogLevel.INFO, member.Username);
                    return true;
                }
            }
        }

        public bool WasEdited(int bookId, DateTime dateTime, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null)
                {
                    logger.Log("Failed to check a book.", LogLevel.WARN, "");
                    return false;
                }

                using (var db = new BookstoreDbContext())
                {
                    var book = db.Books.Find(bookId);
                    if (book == null)
                    {
                        logger.Log("Failed to check a book.", LogLevel.WARN, member.Username);
                        return false;
                    }

                    bool wasEdited = book.LastModified > dateTime;
                    logger.Log($"{member.Username} checked a book for edits.", LogLevel.INFO, member.Username);
                    return wasEdited;
                }
            }
        }

        #endregion

        #region AUTHOR
        public Author CreateAuthor(string firstName, string lastName, string shortDesc, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null || !member.IsAdmin)
                {
                    logger.Log("Failed to create author.", LogLevel.WARN, member?.Username ?? "");
                    return null;
                }

                using (var db = new BookstoreDbContext())
                {
                    if (db.Authors.Any(a => a.FirstName == firstName && a.LastName == lastName))
                    {
                        logger.Log("Failed to create author.", LogLevel.WARN, member.Username);
                        return null;
                    }

                    var author = new Author { FirstName = firstName, LastName = lastName, ShortDesc = shortDesc };
                    db.Authors.Add(author);
                    db.SaveChanges();
                    logger.Log($"{member.Username} successfully created author.", LogLevel.INFO, member.Username);
                    return author;
                }
            }
        }

        public List<Author> GetAuthors()
        {
            lock (locker)
            {
                using (var db = new BookstoreDbContext())
                {
                    logger.Log("Author data queried.", LogLevel.DEBUG, "");
                    return db.Authors.ToList();
                }
            }
        }
        #endregion

        #region USER
        public string LogIn(string username, string password)
        {
            lock (locker)
            {
                try
                {
                    string user = userService.LoginUser(username, password);
                    logger.Log($"User {username} connected.", LogLevel.INFO, username);
                    return user;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    logger.Log($"Failed login attempt for user {username}", LogLevel.WARN, username);
                    return null;
                }
            }
        }

        public void LogOut(string token)
        {
            lock (locker)
            {
                if (userService.IsMemberLoggedIn(token))
                {
                    Member user = userService.GetLoggedInUser(token);
                    userService.LogoutUser(token);
                    logger.Log($"User {user.Username} has disconnected.", LogLevel.INFO, user.Username);
                }
            }
        }

        public bool CreateUser(string firstName, string lastName, string username, string password, bool admin, string token)
        {
            lock (locker)
            {
                Member member = userService.GetLoggedInUser(token);
                if (member == null || !member.IsAdmin)
                {
                    logger.Log($"Failed to create user {username}.", LogLevel.WARN, username);
                    return false;
                }

                using (var db = new BookstoreDbContext())
                {
                    if (db.Members.Find(username) != null)
                    {
                        logger.Log($"Failed to create user {username}.", LogLevel.WARN, username);
                        return false;
                    }

                    var newUser = new Member { Username = username, Password = password, FirstName = firstName, LastName = lastName, IsAdmin = admin };
                    db.Members.Add(newUser);
                    db.SaveChanges();
                    logger.Log($"User {username} created.", LogLevel.INFO, username);
                    return true;
                }
            }
        }

        public bool EditMemberInfo(string firstName, string lastName, string token)
        {
            lock (locker)
            {
                var member = userService.GetLoggedInUser(token);
                if (member == null)
                {
                    logger.Log("Failed to edit member info.", LogLevel.WARN, "");
                    return false;
                }

                using (var db = new BookstoreDbContext())
                {
                    var user = db.Members.FirstOrDefault(m => m.Username == member.Username);
                    if (user == null)
                    {
                        logger.Log("Failed to edit member info.", LogLevel.WARN, member.Username);
                        return false;
                    }

                    user.FirstName = firstName;
                    user.LastName = lastName;
                    db.SaveChanges();
                    logger.Log($"{member.Username} info edited.", LogLevel.DEBUG, member.Username);
                    return true;
                }
            }
        }

        public Member GetMemberInfo(string token)
        {
            lock (locker)
            {
                var member = userService.GetLoggedInUser(token);
                if (member == null)
                {
                    logger.Log("Failed to get member info.", LogLevel.WARN, "");
                    return null;
                }

                using (var db = new BookstoreDbContext())
                {
                    var user = db.Members.FirstOrDefault(m => m.Username == member.Username);
                    logger.Log($"Retrieved info of {member.Username}.", LogLevel.DEBUG, member.Username);
                    return user;
                }
            }
        }
        #endregion
    }
}
