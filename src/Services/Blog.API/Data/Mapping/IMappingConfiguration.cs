using Microsoft.EntityFrameworkCore;

namespace Blog.API.Data.Mapping
{
    public interface IMappingConfiguration
    {
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}
