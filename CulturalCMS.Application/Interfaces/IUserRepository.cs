using CulturalCMS.Application.Common;
using CulturalCMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface IUserRepository: IBaseRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize, List<Expression<Func<User, bool>>> predicates, CancellationToken cancellationToken = default);
    }
}
