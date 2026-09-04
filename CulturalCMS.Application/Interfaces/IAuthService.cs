using CulturalCMS.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserReadOnlyDTO> RegisterUserAsync(UserSignupDTO signupDTO);
    }
}
