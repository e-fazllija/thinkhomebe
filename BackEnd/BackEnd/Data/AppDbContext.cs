using BackEnd.Entities;
using BackEnd.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<RealEstateProperty> RealEstatePropertys { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<RealEstatePropertyPhoto> RealEstatePropertyPhotos { get; set; }
        

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureEntities();

        }
    }
}
