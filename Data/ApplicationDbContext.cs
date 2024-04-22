using Microsoft.EntityFrameworkCore;
using worldCitiesServer.Data.Models;

namespace worldCitiesServer.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }

        public DbSet<City> Cities => Set<City>();
        public DbSet<Country> Countries => Set<Country>();
    }
}
