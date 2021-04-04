using Blog.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.API.Data.Mapping
{
    public class EntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder){}
        public virtual void Configure(EntityTypeBuilder<TEntity> builder) => this.PostConfigure(builder);
        public virtual void ApplyConfiguration(ModelBuilder modelBuilder) => modelBuilder.ApplyConfiguration(this);
    }
}
