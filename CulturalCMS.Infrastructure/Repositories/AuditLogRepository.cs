using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(CulturalDbContext context)
            : base(context)
        {
        }
        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(log => log.User)
                .Where(log => log.EntityName == entityName && log.EntityId == entityId)
                .OrderBy(log => log.Timestamp)
                .ToListAsync(cancellationToken);
        }
    }
}
