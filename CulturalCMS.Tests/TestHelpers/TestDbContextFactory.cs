using CulturalCMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CulturalCMS.Tests.TestHelpers
{
    // Hands out a fresh, isolated InMemory DbContext to each test.
    public static class TestDbContextFactory
    {
        public static CulturalDbContext Create()
        {
            var options = new DbContextOptionsBuilder<CulturalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new CulturalDbContext(options);
        }
    }
}
