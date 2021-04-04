using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Blog.API.Domain.Posts;

namespace Blog.API.Data.Mapping.Posts
{
    public class PostMap : EntityTypeConfiguration<Post>
    {
        public override void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable(nameof(Post));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Body).IsRequired();
            builder.Property(x => x.BodyOverview).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Tags).HasMaxLength(100).IsRequired();
            builder.Property(x => x.MetaTitle).HasMaxLength(100);
            builder.Property(x => x.MetaKeywords).HasMaxLength(50);
            builder.Property(x => x.MetaDescription).HasMaxLength(150);

            builder.Property(x => x.StartDateUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            builder.Property(x => x.CreatedOnUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            builder.Property(x => x.AllowComments).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.Deleted).HasDefaultValue(false).IsRequired();

            base.Configure(builder);
        }
    }
}
