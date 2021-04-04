using Blog.API.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class Repository<T> : IRepository<T> where T : BaseEntity
    {
        #region Fields

        private readonly IDbContext _context;
        private DbSet<T> _entities;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public Repository(IDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(object id)
        {
            return Entities.Find(id);
        }

        /// <summary>
        /// Get entity by identifier as async
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await Entities.FindAsync(id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Add(entity);

            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Insert entity as async
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task InsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Add(entity);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
                Entities.Add(entity);

            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Insert entities as async
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task InsertAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
                Entities.Add(entity);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update entity as async
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update entities as async
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task UpdateAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Remove(entity);

            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete entity as async
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            Entities.Remove(entity);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
                Entities.Remove(entity);

            _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete entities as async
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task DeleteAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            foreach (var entity in entities)
                Entities.Remove(entity);

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return Entities;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return Entities.AsNoTracking();
            }
        }

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        #endregion
    }
}
