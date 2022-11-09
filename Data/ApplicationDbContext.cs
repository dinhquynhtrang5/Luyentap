using Luyentap.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Luyentap.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }

    }

}