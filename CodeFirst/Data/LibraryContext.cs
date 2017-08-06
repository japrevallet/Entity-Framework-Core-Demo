using CodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeFirst.Data
{
    public class LibraryContext: DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {

        }

        public LibraryContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=MNILAY-ENVY\\SQLEXPRESS;Initial Catalog=TutorialsTeam;Integrated Security=True");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Book>().Property(b => b.RowVersion).IsConcurrencyToken();
        }

        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }   
    }
}
