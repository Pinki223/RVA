using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common;
using Common.Model;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IBookstoreService> cf = new ChannelFactory<IBookstoreService>(new NetTcpBinding(), "net.tcp://localhost:9000");
            IBookstoreService proxy = cf.CreateChannel();

            //Console.ReadKey();

            string token = proxy.LogIn("admin", "admin");
            Author author = proxy.CreateAuthor("Ernest dude", "Hamingway", "He is an author", token);
            Console.WriteLine(author.AuthorId.ToString() + " " + author.FirstName);
            //Console.ReadKey();
            proxy.CreateBook("For whom the bell talls", 1980, author.AuthorId, token);
            Dictionary<int, Book> Books = proxy.GetBooks();
            foreach(Book book in Books.Values)
            {
                Console.WriteLine(book.Author.AuthorId.ToString() + " " + book.AuthorId + " " + book.Author.FirstName + " " + book.Title);
            }
            Console.ReadKey();
        }
    }
}
