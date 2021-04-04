using Blog.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public partial interface IDbContext
    {
        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Begin Transactio
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commit Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task CommitTransactionAsync(IDbContextTransaction transaction);

        /// <summary>
        /// Rollback Transaction
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Ensures that the database for the context exists. If it exists, no action is
        /// taken. If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with
        /// the model for this context.
        /// Note that this API does not use migrations to create the database. In addition,
        /// the database that is created cannot be later updated using migrations. If you
        /// are targeting a relational database and using migrations, you can use the DbContext.Database.Migrate()
        /// method to ensure the database is created and all migrations are applied. 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Provides access to database related information and operations for a context.
        /// Instances of this class are typically obtained from Microsoft.EntityFrameworkCore.DbContext.Database
        /// and it is not designed to be directly constructed in your application code.
        /// </summary>
        DatabaseFacade GetDatabase { get; }
    }
}
