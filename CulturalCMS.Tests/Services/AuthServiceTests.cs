using AutoMapper;
using CulturalCMS.Application.BusinessServices;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace CulturalCMS.Tests.Services
{
    // Unit tests for AuthService.RegisterUserAsync (duplicate handling, role assignment, password hashing).
    public class AuthServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IEncryptionUtil _encryptionUtil;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<AuthService>>();
            _encryptionUtil = Substitute.For<IEncryptionUtil>();
            _service = new AuthService(_unitOfWork, _mapper, _logger, _encryptionUtil);
        }

        private static UserSignupDTO CreateDummySignupDTO(string username = "maria")
        {
            return new UserSignupDTO
            {
                Username = username,
                Email = "maria@example.com",
                Password = "PlainPass1!",
                Firstname = "Μαρία",
                Lastname = "Παπαδοπούλου"
            };
        }

        [Fact]
        public async Task RegisterUserAsync_WhenUsernameAlreadyExists_ThrowsEntityAlreadyExistsException()
        {
            // Arrange
            var signupDTO = CreateDummySignupDTO("maria");
            _unitOfWork.UserRepository.GetUserByUsernameAsync("maria")
                .Returns(new User { Id = 1, Username = "maria" });

            // Act & Assert
            await Assert.ThrowsAsync<EntityAlreadyExistsException>(
                () => _service.RegisterUserAsync(signupDTO));

            _encryptionUtil.DidNotReceive().Encrypt(Arg.Any<string>());
            await _unitOfWork.UserRepository.DidNotReceive().AddAsync(Arg.Any<User>());
            await _unitOfWork.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task RegisterUserAsync_WhenValidSignup_AssignsContributorRoleAndSaves()
        {
            // Arrange
            var signupDTO = CreateDummySignupDTO("nikos");
            var mappedUser = new User { Username = "nikos" };

            _unitOfWork.UserRepository.GetUserByUsernameAsync("nikos").Returns((User?)null);
            _unitOfWork.RoleRepository.GetRoleByNameAsync("Contributor")
                .Returns(new Role { Id = 3, Name = "Contributor" });
            _mapper.Map<User>(signupDTO).Returns(mappedUser);
            _encryptionUtil.Encrypt("PlainPass1!").Returns("hashed_pw");

            // Act
            await _service.RegisterUserAsync(signupDTO);

            // Assert
            Assert.Equal(3, mappedUser.RoleId);
            await _unitOfWork.UserRepository.Received(1).AddAsync(mappedUser);
            await _unitOfWork.Received(1).SaveAsync();
        }

        [Fact]
        public async Task RegisterUserAsync_WhenValidSignup_EncryptsPasswordBeforeSaving()
        {
            // Arrange
            var signupDTO = CreateDummySignupDTO("eleni");
            var mappedUser = new User { Username = "eleni" };

            _unitOfWork.UserRepository.GetUserByUsernameAsync("eleni").Returns((User?)null);
            _unitOfWork.RoleRepository.GetRoleByNameAsync("Contributor")
                .Returns(new Role { Id = 3, Name = "Contributor" });
            _mapper.Map<User>(signupDTO).Returns(mappedUser);
            _encryptionUtil.Encrypt("PlainPass1!").Returns("hashed_pw");

            // Act
            await _service.RegisterUserAsync(signupDTO);

            // Assert: the raw password is hashed, and the hash (not the plain text) is what gets persisted.
            _encryptionUtil.Received(1).Encrypt("PlainPass1!");
            Assert.Equal("hashed_pw", mappedUser.Password);
        }

        [Fact]
        public async Task RegisterUserAsync_WhenEmailAlreadyExists_ThrowsEntityAlreadyExistsException()
        {
            // Arrange
            var signupDTO = CreateDummySignupDTO("giorgos");
            _unitOfWork.UserRepository.GetUserByUsernameAsync("giorgos").Returns((User?)null);
            _unitOfWork.UserRepository.GetUserByEmailAsync("maria@example.com")
                .Returns(new User { Id = 2, Email = "maria@example.com" });

            // Act & Assert
            await Assert.ThrowsAsync<EntityAlreadyExistsException>(
                () => _service.RegisterUserAsync(signupDTO));

            _encryptionUtil.DidNotReceive().Encrypt(Arg.Any<string>());
            await _unitOfWork.UserRepository.DidNotReceive().AddAsync(Arg.Any<User>());
            await _unitOfWork.DidNotReceive().SaveAsync();
        }
    }
}
