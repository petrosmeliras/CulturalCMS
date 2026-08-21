using CulturalCMS.Application.Interfaces;
using CulturalCMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CulturalDbContext _context;
        public IAuditLogRepository AuditLogRepository { get; }
        public ICulturalItemRepository CulturalItemRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IUserRepository UserRepository { get; }

        public UnitOfWork(CulturalDbContext context)
        {
            _context = context;
            AuditLogRepository = new AuditLogRepository(context);
            CulturalItemRepository = new CulturalItemRepository(context);
            RoleRepository = new RoleRepository(context);
            UserRepository = new UserRepository(context);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
