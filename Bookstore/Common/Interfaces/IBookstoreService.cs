using Common.Model;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IBookstoreService : IUser, IAuthor, IBooks
    {
        //[OperationContract]
        /// <summary>
        /// Creates a book and automatically creates and assigns a new author.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="publishmentYear"></param>
        /// <param name="author"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        //bool CreateBookAndAuthor(string title, int publishmentYear, Author author, string token);

        [OperationContract]
        /// <summary>
        /// Creates a book and assigns an authorId for an existing author.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="publishmentYear"></param>
        /// <param name="authorId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        int CreateBook(string title, int publishmentYear, int authorId, string token);

        [OperationContract]
        Author CreateAuthor(string firstName, string lastName, string shortDesc, string token);

        [OperationContract]
        bool CreateUser(string firstName, string lastName, string username, string password, bool admin, string token);

        [OperationContract]
        bool DeleteBook(int bookId, string token);

        [OperationContract]
        int CloneBook(Book book, string token);
        [OperationContract]
        bool WasEdited(int bookId, DateTime date, string token);

        [OperationContract]
        bool EditBook(int bookId, string title, int publishYear, int authorId, string token);

        [OperationContract]
        Dictionary<int, Book> GetBooks();

        [OperationContract]
        Member GetMemberInfo(string token);

        [OperationContract]
        bool EditMemberInfo(string firstName, string lastName, string token);

        [OperationContract]
        string LogIn(string username, string password);

        [OperationContract]
        void LogOut(string token);

        [OperationContract]
        bool LeaseBook(int bookId, string token);

        [OperationContract]
        bool ReturnBook(int bookId, string token);

        [OperationContract]
        List<Author> GetAuthors();

    }
}
