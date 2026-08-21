using CulturalCMS.Application.Common;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CulturalDbContext context) : base(context)
        {
        }

        public override async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async  Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u  => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates, CancellationToken cancellationToken = default)
        {
            int totalRecords;
            IQueryable<User> query = _context.Users.Include(u => u.Role);

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // υπονοείται το AND
                }
            }
            totalRecords = await query.CountAsync(cancellationToken);
            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Id) // Πάντα OrderBy για να διασφαλίσουμε την σταθερή σειρά των αποτελεσμάτων
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<User>()
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }
    }
}