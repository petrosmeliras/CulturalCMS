using CulturalCMS.Application.Common;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.DTO.Filters;
using CulturalCMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserReadOnlyDTO> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserReadOnlyDTO> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserReadOnlyDTO> UpdateUserRoleAsync(int userId, string newRoleName, CancellationToken cancellationToken = default);
        Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize, UserFiltersDTO userFiltersDTO, CancellationToken cancellationToken = default);
        Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials);
        string CreateUserToken(User user);
    }
}
