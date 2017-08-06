using CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirst.Data
{
    public class DBInitialize
    {
        public static void Initialize(LibraryContext context)
        {
            context.Database.EnsureCreated();

            if(!context.Author.Any())
            {
                // Add Default Author data
                var authors = new List<Author>()
                {
                    new Author { Name = "First Author", MailId = "First@Author.com" },
                    new Author { Name = "Second Author", MailId="Second@Author.com" },
                    new Author { Name = "Third Author", MailId="Third@Author.com" }
                };
                context.Author.AddRange(authors);
                context.SaveChanges();
            }

            if(!context.Book.Any())
            {
                // Add default books
                var books = new List<Book>()
                {
                    new Book { AuthorId = 1, Title = "First Book",
                        Description = "Desc of 1st Book", Price = 100 },
                    new Book { AuthorId = 2, Title = "Second Book",
                        Description = "Desc of 2nd Book", Price = 200 },
                    new Book { AuthorId = 3, Title = "Third Book",
                        Description = "Desc of 3rd Book", Price = 300 },
                };
                context.Book.AddRange(books);
                context.SaveChanges();
            }
        }
    }
}
