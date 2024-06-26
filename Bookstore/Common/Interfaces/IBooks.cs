using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Model;

namespace Common
{
    public interface IBooks
    {
        Dictionary<int, Book> GetBooks();
        int CreateBook(string title, int publishmentYear, int authorId, string token);
        bool DeleteBook(int bookId, string token);
        int CloneBook(Book book, string token);
        bool EditBook(int bookId, string title, int publishYear, int authorId, string token);
        bool LeaseBook(int bookId, string token);
        bool ReturnBook(int bookId, string token);
        bool WasEdited(int bookId, DateTime date, string token);
    }
}
