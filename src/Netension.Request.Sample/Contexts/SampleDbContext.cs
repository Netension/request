using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Netension.Request.Sample.Contexts
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext([NotNull] DbContextOptions options)
            : base(options)
        {
        }
    }
}
