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
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(CulturalDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == roleName);
        }
    }
}
