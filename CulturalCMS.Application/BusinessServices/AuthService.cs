using AutoMapper;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Constants;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.BusinessServices
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IEncryptionUtil _encryptionUtil;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AuthService> logger,
            IEncryptionUtil encryptionUtil)

        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _encryptionUtil = encryptionUtil;
        }

        public async Task<UserReadOnlyDTO> RegisterUserAsync(UserSignupDTO signupDTO)
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(signupDTO.Username!);
            if (existingUser != null)
            {
                throw new EntityAlreadyExistsException("User", $"User with username '{signupDTO.Username}' already exists.");
            }

            var existingEmail = await _unitOfWork.UserRepository.GetUserByEmailAsync(signupDTO.Email!);
            if (existingEmail != null)
            {
                throw new EntityAlreadyExistsException("User", $"User with email '{signupDTO.Email}' already exists.");
            }

            var contributorRole = await _unitOfWork.RoleRepository.GetRoleByNameAsync(AppRoles.Contributor);
            if (contributorRole == null)
            {
                throw new InvalidOperationException("Default role 'Contributor' is missing from the database.");
            }

            var user = _mapper.Map<User>(signupDTO);

            user.RoleId = contributorRole.Id;

            user.Password = _encryptionUtil.Encrypt(signupDTO.Password!);

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("New user registered with username: {Username}", user.Username);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }
    }
}
