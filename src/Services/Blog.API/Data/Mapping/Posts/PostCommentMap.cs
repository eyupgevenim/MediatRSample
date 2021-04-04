using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Blog.API.Domain.Posts;

namespace Blog.API.Data.Mapping.Posts
{
    public class PostCommentMap : EntityTypeConfiguration<PostComment>
    {
        public override void Configure(EntityTypeBuilder<PostComment> builder)
        {
            builder.ToTable(nameof(PostComment));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CommentText).HasMaxLength(500).IsRequired();
            builder.Property(x => x.IsApproved).HasDefaultValue(true).IsRequired();

            builder.HasOne(x => x.Post)
                .WithMany(x => x.PostComments)
                .HasForeignKey(x => x.PostId);

            base.Configure(builder);
        }
    }
}
