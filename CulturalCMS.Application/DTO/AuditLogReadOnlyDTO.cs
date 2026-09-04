using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.DTO
{
    public record AuditLogReadOnlyDTO
    {
        public int Id { get; init; }
        public string Action { get; init; } = string.Empty;       
        public string EntityName { get; init; } = string.Empty;   
        public int EntityId { get; init; }
        public int UserId { get; init; }                          
        public string Username { get; init; } = string.Empty;     
        public DateTime Timestamp { get; init; }
        public string? OldValues { get; init; }                   
        public string? NewValues { get; init; }
        public string? ChangedColumns { get; init; }
    }
}
