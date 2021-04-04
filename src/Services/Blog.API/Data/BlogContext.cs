using Blog.API.Data.Mapping;
using Blog.API.Domain;
using Blog.API.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    public partial class BlogContext : DbContext, IDbContext
    {
        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
        public bool HasActiveTransaction => _currentTransaction != null;

        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        public BlogContext(DbContextOptions<BlogContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            System.Diagnostics.Debug.WriteLine("BlogContext::ctor ->" + this.GetHashCode());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //dynamically load all entity and query type configurations
            var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => (type.BaseType?.IsGenericType ?? false)
                && (type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)));

            foreach (var typeConfiguration in typeConfigurations)
            {
                var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                configuration.ApplyConfiguration(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity => base.Set<TEntity>();

        /// <summary>
        /// Save Entities
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public new async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        /// <summary>
        /// Begin Transaction
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) 
                return null;

            _currentTransaction = await GetDatabase.BeginTransactionAsync(cancellationToken);
            return _currentTransaction;
        }

        /// <summary>
        /// Commit Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await base.SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Rollback Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

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
        public virtual async Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default) => await GetDatabase.EnsureCreatedAsync(cancellationToken);

        /// <summary>
        /// Provides access to database related information and operations for a context.
        /// Instances of this class are typically obtained from Microsoft.EntityFrameworkCore.DbContext.Database
        /// and it is not designed to be directly constructed in your application code.
        /// </summary>
        public virtual DatabaseFacade GetDatabase => base.Database;
    }
}
