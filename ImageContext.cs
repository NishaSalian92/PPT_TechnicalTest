// Data/ImageContext.cs

using Microsoft.EntityFrameworkCore;
using ImageBackend.Models;

namespace ImageBackend.Data
{
    public class ImageContext : DbContext
    {
        public ImageContext(DbContextOptions<ImageContext> options) : base(options) { }

        public DbSet<Image> Images { get; set; }
    }
}
