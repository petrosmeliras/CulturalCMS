using CulturalCMS.API;
using CulturalCMS.Application.Common;
using CulturalCMS.Application.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace CulturalCMS.Tests.Integration
{
    // Integration tests that exercise the search endpoints through the full HTTP pipeline.
    public class CulturalItemsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CulturalItemsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Search_WhenCalledWithNoFilters_ReturnsOkWithPaginatedResult()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/cultural-items/search");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<CulturalItemReadOnlyDTO>>();

            Assert.NotNull(result);
            Assert.True(result!.PageNumber >= 1);
            Assert.True(result.PageSize > 0);
        }

        [Fact]
        public async Task Search_WhenCalledWithCategoryFilter_ReturnsOnlyMatchingCategory()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/cultural-items/search?category=Γλυπτό");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<CulturalItemReadOnlyDTO>>();

            Assert.NotNull(result);
            Assert.All(result!.Data, item => Assert.Contains("Γλυπτό", item.Category));
        }

        [Fact]
        public async Task SearchAllStatuses_WhenCalledWithoutAuthentication_ReturnsForbiddenOrUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/cultural-items/search/all");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
                $"Expected 401 or 403, but got {response.StatusCode}");
        }
    }
}
