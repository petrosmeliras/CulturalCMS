using CulturalCMS.Application.Common;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.SearchQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface ICulturalItemService
    {
        Task<IEnumerable<CulturalItemReadOnlyDTO>> GetAllItemsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<CulturalItemReadOnlyDTO>> GetPublishedItemsAsync(CancellationToken cancellationToken = default);
        Task<CulturalItemReadOnlyDTO> GetItemByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PaginatedResult<CulturalItemReadOnlyDTO>> SearchMyItemsAsync(ItemSearchQuery query, int userId, CancellationToken cancellationToken = default);
        Task<CulturalItemReadOnlyDTO> CreateItemAsync(CulturalItemCreateDTO createDTO, int userId);
        Task<PaginatedResult<CulturalItemReadOnlyDTO>> SearchItemsAsync(ItemSearchQuery query, bool isPrivileged,  CancellationToken cancellationToken = default);
        Task UpdateItemAsync(int id, CulturalItemUpdateDTO updateDTO, int userId, string userRole);
        Task DeleteItemAsync(int id, int userId, string userRole);

        Task SubmitItemAsync(int id, int userId, string userRole);
        Task ApproveItemAsync(int id, int userId, string userRole);
        Task RejectItemAsync(int id, int userId, string userRole);
    }
}
