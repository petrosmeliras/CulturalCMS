using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface IApplicationService
    {
        IUserService UserService { get; }
        ICulturalItemService CulturalItemService { get; }
        IAuditLogService AuditLogService { get; }
        IAuthService AuthService { get; }
        IImageService ImageService { get; }
    }
}
