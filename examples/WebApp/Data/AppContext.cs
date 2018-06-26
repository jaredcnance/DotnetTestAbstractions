using Microsoft.EntityFrameworkCore;
using WebApp.Articles;

namespace WebApp.Data
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options)
           : base(options)
        { }

        public DbSet<Article> Articles { get; set; }
    }
}