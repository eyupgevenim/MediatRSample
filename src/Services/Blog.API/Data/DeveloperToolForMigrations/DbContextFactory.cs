using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Blog.API.Data.DeveloperToolForMigrations
{
    /// <summary>
    /// for migrations scripts:
    ///_> dotnet ef migrations add Initial
    ///_> dotnet ef database update
    ///_>#dotnet ef database update Initial
    ///
    /// Add-Migration Initial -OutputDir "Data/Migrations"
    /// dotnet ef migrations add Initial -o "Data/Migrations"
    /// 
    /// dotnet ef migrations add Initial --context BlogContext -o "Data/Migrations"
    /// </summary>
    public class DbContextFactory : IDesignTimeDbContextFactory<BlogContext>
    {
        public BlogContext CreateDbContext(string[] args)
        {
            //Database connection string
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=blog_api_db;Trusted_Connection=True;MultipleActiveResultSets=true";
            var builder = new DbContextOptionsBuilder<BlogContext>();
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("Blog.API"));
            return new BlogContext(builder.Options);
            
            //var builder = new DbContextOptionsBuilder<BlogContext>();
            //builder.UseInMemoryDatabase("MyInMemoryDatabase");
            //return new BlogContext(builder.Options);
        }
    }
}
