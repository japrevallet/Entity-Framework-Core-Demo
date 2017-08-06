using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataFirst.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DataFirst.Controllers
{
    public class HomeController : Controller
    {
        private TutorialsTeamContext _context = null;
        public HomeController(TutorialsTeamContext context)
        {
            _context = context;
        }

        public string Index()
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
    }
}
