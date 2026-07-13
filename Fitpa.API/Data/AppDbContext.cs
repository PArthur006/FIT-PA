using Fitpa.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Fitpa.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Pesagem> Pesagens { get; set; }
    }
}