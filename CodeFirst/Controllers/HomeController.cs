using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CodeFirst.Models;
using CodeFirst.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;

namespace CodeFirst.Controllers
{
    public class HomeController : Controller
    {
        private LibraryContext _context;

        public HomeController(LibraryContext context)
        {
            _context = context;
        }

        public string Index()
        {
            _context.Database.EnsureCreated();
            return "DB created";
        }

        public string Drop()
        {
            _context.Database.EnsureDeleted();
            return "DB deleted";
        }

        public string CreateAuthor(Author a)
        {
            try
            {
                if (a.Name != null && a.MailId != null)
                {
                    _context.Add(a);
                    _context.SaveChanges();
                    return "Author details added successfully";
                }
                else
                    return "Missing some properties!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateBook(Book b)
        {
            try
            {
                if (b.Title != null && b.Price != 0 && b.Description != null && b.AuthorId != 0)
                {
                    _context.Book.Add(b);
                    _context.SaveChanges();
                    return "Book details added successfully";
                }
                else
                    return "Missing some properties!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ReadBook()
        {
            var books = _context.Book.Include("Author").AsNoTracking();
            StringBuilder sb = new StringBuilder();
            if (books == null)
                return "Not found any books!";
            else
            {
                foreach (var book in books)
                {
                    sb.Append($"Book Id: {book.Id}\r\n");
                    sb.Append($"Title: {book.Title}\r\n");
                    sb.Append($"Description: {book.Description}\r\n");
                    sb.Append($"Price: {book.Price}\r\n");
                    sb.Append($"Author Id: {book.AuthorId}\r\n");
                    sb.Append($"Author Name: {book.Author.Name}\r\n");
                    sb.Append($"Author Mail Id: {book.Author.MailId}\r\n");
                    sb.Append("##################################################\r\n");
                }
                return sb.ToString();
            }
        }

        public string BookDetails(int? bookId)
        {
            if (bookId == null)
                return "Enter Book Id!";

            var result = _context.Book.Include("Author").SingleOrDefaultAsync(b => b.Id == bookId);
            StringBuilder sb = new StringBuilder();
            var book = result.Result;

            if (book == null)
                return "Not found book with given Id!";
            else
            {
                sb.Append($"Book Id: {book.Id}\r\n");
                sb.Append($"Title: {book.Title}\r\n");
                sb.Append($"Description: {book.Description}\r\n");
                sb.Append($"Price: {book.Price}\r\n");
                sb.Append($"Author Id: {book.AuthorId}\r\n");
                sb.Append($"Author Name: {book.Author.Name}\r\n");
                sb.Append($"Author Mail Id: {book.Author.MailId}\r\n");
                return sb.ToString();
            }
        }

        public string UpdateBook(int bookId, Book newBook)
        {
            try
            {
                if (bookId != newBook.Id)
                    return "Invalid data!";
                else
                {
                    if (newBook.Title != null && newBook.Price != 0 && newBook.Description != null && newBook.AuthorId != 0)
                    {
                        //_context.Update(newBook);
                        //_context.SaveChanges();
                        //return "Book details updated successfully";

                        var dbBook = _context.Book.SingleOrDefaultAsync(book => book.Id == bookId);
                        dbBook.Result.Title = newBook.Title;
                        dbBook.Result.Description = newBook.Description;
                        dbBook.Result.Price = newBook.Price;
                        dbBook.Result.AuthorId = newBook.AuthorId;
                        _context.SaveChanges();
                        return "Updated successfully";
                    }
                    else
                        return "Missing some properties";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DeleteBook(int bookId)
        {
            try
            {
                if (bookId == 0)
                    return "Enter Book Id";
                else
                {
                    var result = _context.Book.SingleOrDefaultAsync(b => b.Id == bookId);
                    var book = result.Result;
                    if (book == null)
                        return "Book with given Id does not exist";
                    else
                    {
                        _context.Book.Remove(book);
                        _context.SaveChanges();
                        return "Book deleted successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string PrintAsString(Book book)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Book Id: {book.Id}\r\n");
            sb.Append($"Title: {book.Title}\r\n");
            sb.Append($"Description: {book.Description}\r\n");
            sb.Append($"Price: {book.Price}\r\n");
            sb.Append($"Author Id: {book.AuthorId}\r\n");
            sb.Append($"Author Name: {book.Author.Name}\r\n");
            sb.Append($"Author Mail Id: {book.Author.MailId}\r\n");
            return sb.ToString();
        }

        public string SortBookDetails(string columnName)
        {
            try
            {
                var books = _context.Book.Include("Author").AsNoTracking();
                if (columnName != null)
                {
                    switch (columnName.ToLower())
                    {
                        case "id":
                            books = books.OrderBy(b => b.Id);
                            break;
                        case "price":
                            books = books.OrderBy(b => b.Price);
                            break;
                        case "author":
                            books = books.OrderBy(b => b.Author.Name);
                            break;
                        case "title":
                            books = books.OrderBy(b => b.Title);
                            break;
                    }
                }

                StringBuilder data = new StringBuilder();
                foreach (var book in books)
                {
                    data.AppendLine(PrintAsString(book));
                }
                return data.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string FilterBookDetails(string columnName, string value)
        {
            try
            {
                var books = _context.Book.Include("Author").AsNoTracking();

                if (columnName != null && value != null)
                {
                    switch (columnName.ToLower())
                    {
                        case "id":
                            int id = Convert.ToInt32(value);
                            books = books.Where(b => b.Id == id);
                            break;
                        case "price":
                            double price = Convert.ToDouble(value);
                            books = books.Where(b => b.Price == price);
                            break;
                        case "author":
                            books = books.Where(b => b.Author.Name == value);
                            break;
                        case "title":
                            books = books.Where(b => b.Title == value);
                            break;
                    }

                    StringBuilder data = new StringBuilder();
                    foreach (var book in books)
                    {
                        data.AppendLine(PrintAsString(book));
                    }
                    return data.ToString();
                }
                else
                    return "Missing columnName/value";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string EntityState()
        {
            var book = new Book
            {
                Title = "New Added Book",
                Description = "Long Desc about new Book",
                Price = 320,
                AuthorId = 1
            };
            //_context.Book.Add(book);
            //return _context.Entry(book).State.ToString();

            _context.Entry(book).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            _context.SaveChanges();
            return "Book Added";
        }

        public string AttachEntity(Book book)
        {
            string msg = string.Empty;
            if (book.Id > 0)
            {
                _context.Book.Attach(book);
                _context.Entry(book).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                msg = "Updated";
            }
            else
            {
                _context.Book.Add(book);
                msg = "Added";
            }

            _context.SaveChanges();
            return $"Book details {msg} successfully.";
        }

        public string EntryState()
        {
            var book = _context.Book.Find(1);
            book.Title = "First Book";
            var entry = _context.Entry(book);

            StringBuilder data = new StringBuilder();
            data.AppendLine($"Entity State: {entry.State}");
            data.AppendLine($"Entity Name: {entry.Entity.GetType().FullName}");
            data.AppendLine($"Original Title: {entry.OriginalValues["Title"]}");
            data.AppendLine($"Currnet Value: {entry.CurrentValues["Title"]}");
            return data.ToString();
        }

        public string NativeStatements()
        {
            //var books = _context.Book.FromSql("SELECT * FROM Book WHERE Price > 200").ToList();
            //StringBuilder data = new StringBuilder();
            //foreach (var book in books)
            //{
            //    data.Append($"Title: {book.Title} | Price: {book.Price}");
            //}
            //return data.ToString();

            //int result = _context.Database.ExecuteSqlCommand("DELETE FROM Book WHERE Id = 1");
            //if (result >= 0)
            //    return "Deleted Successfully";
            //else
            //    return "Not found book with given Id";

            DbConnection con = _context.Database.GetDbConnection();
            con.Open();
            DbCommand com = con.CreateCommand();
            com.CommandText = "SELECT Name FROM Author";
            com.CommandType = System.Data.CommandType.Text;

            DbDataReader dr = com.ExecuteReader();
            StringBuilder data = new StringBuilder();
            while (dr.Read())
            {
                data.AppendLine(dr.GetString(0));
            }
            return data.ToString();
        }

        public string SPDemo(int id)
        {
            try
            {
                //var books = _context.Book.FromSql("GetBooks @Id", new SqlParameter("@Id", id)).ToList();
                //StringBuilder data = new StringBuilder();
                //foreach (var book in books)
                //{
                //    data.AppendLine(book.Title);
                //}
                //return data.ToString();

                if (id <= 0)
                    return "Enter Book Id";
                else
                {
                    var con = _context.Database.GetDbConnection();
                    con.Open();
                    var comm = con.CreateCommand();
                    comm.CommandText = "DeleteBooks";
                    comm.CommandType = System.Data.CommandType.StoredProcedure;
                    comm.Parameters.Add(new SqlParameter("@Id", id));

                    int result = comm.ExecuteNonQuery();
                    if (result > 0)
                        return "Deleted successfully";
                    else
                        return "Book with given Id does not exist";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Migrate()
        {
            _context.Database.Migrate();
            return "DB Migrated";
        }

        public string NormalQuery()
        {
            int[] bookIds = new int[] { 1, 2, 3 };
            StringBuilder data = new StringBuilder();
            foreach (var bookId in bookIds)
            {
                Book book = _context.Book.SingleOrDefault(b => b.Id == bookId);
                data.AppendLine(book.Title);
            }
            return data.ToString();
        }

        public string CompiledQuery()
        {
            int[] bookIds = new int[] { 1, 2, 3 };
            StringBuilder data = new StringBuilder();
            var query = EF.CompileQuery((LibraryContext context, int id) =>
                context.Book.SingleOrDefault(b => b.Id == id)
            );
            foreach (var bookId in bookIds)
            {
                Book book = query(_context, bookId);
                data.AppendLine(book.Title);
            }
            return data.ToString();
        }

        public string SearchBook(string title)
        {
            StringBuilder data = new StringBuilder();
            var filterdBook = _context.Book.Where(b =>
                EF.Functions.Like(b.Title, $"%{title}%")
            ).Include("Author");
            foreach (Book book in filterdBook)
            {
                data.AppendLine(PrintAsString(book));
            }
            return data.ToString();
        }

        public string InterpolationDemo(string title)
        {
            StringBuilder data = new StringBuilder();
            var filteredBooks = _context.Book.FromSql($"SELECT * FROM Book WHERE Title LIKE '%{title}%'");
            foreach (Book book in filteredBooks)
            {
                data.AppendLine(book.Title);
            }
            return data.ToString();
        }

        public string PerformConcurrency()
        {
            try
            {
                var option = new DbContextOptionsBuilder<LibraryContext>().UseSqlServer("Data Source=MNILAY-ENVY\\SQLEXPRESS;Initial Catalog=TutorialsTeam;Integrated Security=True").Options;
                LibraryContext _context_new = new LibraryContext(option);

                Book b1 = _context.Book.Find(1);
                b1.Title = "Title using Context1";
                Book b2 = _context_new.Book.Find(1);
                b2.Title = "Title using Context2";

                _context.SaveChanges();
                _context_new.SaveChanges();
                return "Changed Both";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string PerformTransaction()
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.Book.Add(new Book { Title = "Trans_1", AuthorId = 1, Description = "1st Book using transaction", Price = 20 });
                    _context.SaveChanges();
                    _context.Book.Add(new Book { Title = "Trans_2", AuthorId = 2, Description = "2st Book using transaction", Price = 30 });
                    _context.SaveChanges();
                    transaction.Commit();
                }
                return "Transaction completed";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
