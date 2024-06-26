using Common.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreBackend.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BookstoreBackend.BookstoreDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BookstoreBackend.BookstoreDbContext context)
        {
            context.Members.AddOrUpdate(new Member() { FirstName = "Admin", LastName = "Adminic", Username = "admin", Password = "admin", IsAdmin = true });

            context.Members.AddOrUpdate(new Member() { FirstName = "Testni", LastName = "Klijent", Username = "test", Password = "test", IsAdmin = false });

            IList<Author> Authors = new List<Author>();
            //Authors.Add(new Author() { AuthorId = 1, FirstName = "Ivo", LastName = "Andrić", ShortDesc = "Ivo Andrić bio je srpski književnik i diplomata Kraljevine Jugoslavije. Godine 1961. dobio je Nobelovu nagradu za književnost „za epsku snagu kojom je oblikovao teme i prikazao sudbine ljudi tokom istorije svoje zemlje”" });
            //Authors.Add(new Author() { AuthorId = 2, FirstName = "Dimitri", LastName = "Gluhovski", ShortDesc = "Dimitrij Aleksejevič Gluhovski je ruski pisac i novinar. Najpoznatiji je po seriji romana naučne fantastike Metro." });
            foreach (var author in Authors)
            {
                context.Authors.AddOrUpdate(author);
            }

            IList<Book> Books = new List<Book>();
            //Books.Add(new Book() { BookId = 1, Author = 1, Member = string.Empty, PublishYear = 1945, Title = "Na Drini ćuprija" });
            //Books.Add(new Book() { BookId = 2, Author = 1, Member = string.Empty, PublishYear = 1954, Title = "Prokleta avlija" });
            //Books.Add(new Book() { BookId = 3, Author = 2, Member = string.Empty, PublishYear = 2002, Title = "Metro 2033" });
            //Books.Add(new Book() { BookId = 4, Author = 2, Member = string.Empty, PublishYear = 2009, Title = "Metro 2034" });
            foreach(var book in Books)
            {
                context.Books.AddOrUpdate(book);
            }

            context.SaveChanges();
        }
    }
}
