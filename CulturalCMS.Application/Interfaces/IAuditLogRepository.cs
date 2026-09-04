using CulturalCMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface IAuditLogRepository 
    {
        Task AddAsync(AuditLog entity);
        Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int entityId, CancellationToken cancellationToken = default);
    }
}
