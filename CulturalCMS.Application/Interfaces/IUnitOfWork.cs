using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IAuditLogRepository AuditLogRepository { get; }
        ICulturalItemRepository CulturalItemRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }

        Task<bool> SaveAsync(); 
    }
}
