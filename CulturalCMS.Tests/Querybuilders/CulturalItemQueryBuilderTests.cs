using CulturalCMS.Application.SearchQueries;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Enums;
using CulturalCMS.Infrastructure.Data;
using CulturalCMS.Infrastructure.QueryBuilders;
using CulturalCMS.Tests.TestHelpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CulturalCMS.Tests.QueryBuilders
{
    // Unit tests for the search query builder's filtering and sorting logic.
    // Uses a real InMemory DbContext because the point is to test actual LINQ execution.
    public class CulturalItemQueryBuilderTests
    {
        private readonly CulturalDbContext _context;

        public CulturalItemQueryBuilderTests()
        {
            _context = TestDbContextFactory.Create();
        }

        private static CulturalItem CreateItem(string title, string category, ItemStatus status, int viewCount = 0)
        {
            return new CulturalItem
            {
                Title = title,
                Description = "Test description",
                Category = category,
                HistoricalPeriod = "Classical Period",
                Status = status,
                ViewCount = viewCount,
                CreatedById = 1,
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task ApplyFilters_WhenCategoryGiven_ReturnsOnlyMatchingItems()
        {
            // Arrange
            _context.CulturalItems.AddRange(
                CreateItem("Δίσκος της Φαιστού", "Γλυπτό", ItemStatus.Published),
                CreateItem("Αμφορέας", "Αγγειοπλαστική", ItemStatus.Published)
            );
            await _context.SaveChangesAsync();

            var query = new ItemSearchQuery { Category = "Γλυπτό" };

            // Act
            var result = CulturalItemQueryBuilder.ApplyFilters(_context.CulturalItems, query).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Δίσκος της Φαιστού", result[0].Title);
        }

        [Fact]
        public async Task ApplyFilters_WhenStatusGiven_ReturnsOnlyMatchingStatus()
        {
            // Arrange
            _context.CulturalItems.AddRange(
                CreateItem("Draft Item", "Γλυπτό", ItemStatus.Draft),
                CreateItem("Published Item", "Γλυπτό", ItemStatus.Published)
            );
            await _context.SaveChangesAsync();

            var query = new ItemSearchQuery { Status = "Published" };

            // Act
            var result = CulturalItemQueryBuilder.ApplyFilters(_context.CulturalItems, query).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(ItemStatus.Published, result[0].Status);
        }

        [Fact]
        public async Task ApplyFilters_WhenMetadataKeyAndValueGiven_ReturnsOnlyItemsWithMatchingMetadataPair()
        {
            // Arrange
            var itemWithBronze = CreateItem("Αγαλματίδιο", "Γλυπτό", ItemStatus.Published);
            itemWithBronze.Metadata.Add(new ItemMetadata { Key = "Υλικό", Value = "Χαλκός" });

            var itemWithMarble = CreateItem("Άγαλμα", "Γλυπτό", ItemStatus.Published);
            itemWithMarble.Metadata.Add(new ItemMetadata { Key = "Υλικό", Value = "Μάρμαρο" });

            _context.CulturalItems.AddRange(itemWithBronze, itemWithMarble);
            await _context.SaveChangesAsync();

            var query = new ItemSearchQuery { MetadataKey = "Υλικό", MetadataValue = "Χαλκός" };

            // Act
            var result = CulturalItemQueryBuilder.ApplyFilters(_context.CulturalItems, query).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Αγαλματίδιο", result[0].Title);
        }

        [Fact]
        public async Task ApplyFilters_WhenOnlyMetadataValueGiven_MatchesAcrossAnyKey()
        {
            // Arrange
            var item = CreateItem("Θέατρο Επιδαύρου", "Αρχιτεκτονική", ItemStatus.Published);
            item.Metadata.Add(new ItemMetadata { Key = "Tag", Value = "Θέατρο" });

            var unrelatedItem = CreateItem("Άσχετο Έκθεμα", "Αγγειοπλαστική", ItemStatus.Published);

            _context.CulturalItems.AddRange(item, unrelatedItem);
            await _context.SaveChangesAsync();

            var query = new ItemSearchQuery { MetadataValue = "Θέατρο" };

            // Act
            var result = CulturalItemQueryBuilder.ApplyFilters(_context.CulturalItems, query).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Θέατρο Επιδαύρου", result[0].Title);
        }

        [Fact]
        public void ApplySorting_WhenSortByTitleAscending_ReturnsItemsInAlphabeticalOrder()
        {
            // Arrange
            var items = new[]
            {
                CreateItem("Ζ Έκθεμα", "Γλυπτό", ItemStatus.Published),
                CreateItem("Α Έκθεμα", "Γλυπτό", ItemStatus.Published),
                CreateItem("Μ Έκθεμα", "Γλυπτό", ItemStatus.Published)
            }.AsQueryable();

            // Act
            var result = CulturalItemQueryBuilder.ApplySorting(items, sortBy: "Title", sortOrder: "asc").ToList();

            // Assert
            Assert.Equal("Α Έκθεμα", result[0].Title);
            Assert.Equal("Ζ Έκθεμα", result[1].Title);
            Assert.Equal("Μ Έκθεμα", result[2].Title);
        }

        [Fact]
        public void ApplySorting_WhenSortByViewCountDescending_ReturnsMostViewedFirst()
        {
            // Arrange
            var items = new[]
            {
                CreateItem("Low Views", "Γλυπτό", ItemStatus.Published, viewCount: 5),
                CreateItem("High Views", "Γλυπτό", ItemStatus.Published, viewCount: 100),
                CreateItem("Mid Views", "Γλυπτό", ItemStatus.Published, viewCount: 40)
            }.AsQueryable();

            // Act
            var result = CulturalItemQueryBuilder.ApplySorting(items, sortBy: "ViewCount", sortOrder: "desc").ToList();

            // Assert
            Assert.Equal("High Views", result[0].Title);
            Assert.Equal("Mid Views", result[1].Title);
            Assert.Equal("Low Views", result[2].Title);
        }
    }
}
