using CulturalCMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.BusinessServices
{
    public class ApplicationService : IApplicationService
    {
        public IUserService UserService { get; }

        public ICulturalItemService CulturalItemService { get; }

        public IAuditLogService AuditLogService { get; }

        public IAuthService AuthService { get; }
        public IImageService ImageService { get; }

        public ApplicationService(IUserService userService, ICulturalItemService culturalItemService, IAuditLogService auditLogService, IAuthService authService, IImageService imageService)
        {
            UserService = userService;
            CulturalItemService = culturalItemService;
            AuditLogService = auditLogService;
            AuthService = authService;
            ImageService = imageService;
        }
    }
}
