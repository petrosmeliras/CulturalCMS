using AutoMapper;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.BusinessServices
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuditLogService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<IEnumerable<AuditLogReadOnlyDTO>> GetLogsByEntityAsync(string entityName, int entityId, CancellationToken cancellationToken = default)
        {
            var logs = await _unitOfWork.AuditLogRepository.GetLogsByEntityAsync(entityName, entityId, cancellationToken);
            _logger.LogInformation("Retrieved {Count} audit logs for {EntityName} with id {EntityId}", logs.Count(), entityName, entityId);

            return _mapper.Map<IEnumerable<AuditLogReadOnlyDTO>>(logs);
        }
    }
}
