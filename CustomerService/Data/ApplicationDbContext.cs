using CustomerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Customer> ASCustomers { get; set; }
        public DbSet<User> ASUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}
