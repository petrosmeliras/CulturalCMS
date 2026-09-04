using CulturalCMS.Application.Common;
using CulturalCMS.Application.SearchQueries;
using CulturalCMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface ICulturalItemRepository : IBaseRepository<CulturalItem>
    {
        Task<CulturalItem?> GetItemWithMetadataAsync(int id, CancellationToken cancellationToken = default);
        Task<PaginatedResult<CulturalItem>> SearchAsync(ItemSearchQuery query, int? ownerId = null, CancellationToken cancellationToken = default);
        Task ViewCountAsync(int id, CancellationToken cancellationToken = default);

    }
}
