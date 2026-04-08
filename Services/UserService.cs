using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;
using Moq;
using NUnit.Framework;
using RepositoryContracts;
using Services;

namespace UnitTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private UserService _service;

        private const string ValidPassword = "ValidPass1!";

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _service = new UserService(_repositoryMock.Object);
        }

        #region CreateAsync

        [Test]
        public async Task CreateAsync_ValidUser_ReturnsCreatedUser()
        {
            var user = new User
            {
                Name = "John",
                Username = "john123",
                Email = "john@test.com",
                Password = ValidPassword
            };

            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            var result = await _service.CreateAsync(user);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(user.Username));

            _repositoryMock.Verify(r => r.CreateAsync(It.Is<User>(u =>
                u.Password != ValidPassword // ensure hashing happened
            )), Times.Once);
        }

        [Test]
        public void CreateAsync_InvalidPassword_ThrowsArgumentException()
        {
            var user = new User
            {
                Name = "John",
                Username = "john123",
                Email = "john@test.com",
                Password = "weak"
            };

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(user));
        }

        [Test]
        public void CreateAsync_EmailAlreadyExists_ThrowsCorrectException()
        {
            var user = new User
            {
                Name = "John",
                Username = "john123",
                Email = "john@test.com",
                Password = ValidPassword
            };

            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new InvalidOperationException("User with email already exists"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(user));

            Assert.That(ex!.Message, Does.Contain(user.Email));
        }

        [Test]
        public void CreateAsync_UsernameAlreadyExists_ThrowsCorrectException()
        {
            var user = new User
            {
                Name = "John",
                Username = "john123",
                Email = "john@test.com",
                Password = ValidPassword
            };

            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new InvalidOperationException("User with username already exists"));

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(user));

            Assert.That(ex!.Message, Does.Contain(user.Username));
        }

        #endregion

        #region GetByEmailAsync

        [Test]
        public async Task GetByEmailAsync_UserExists_ReturnsUser()
        {
            var email = "john@test.com";
            var user = new User { Email = email };

            _repositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            var result = await _service.GetByEmailAsync(email);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(email));
        }

        [Test]
        public void GetByEmailAsync_InvalidEmailFormat_Throws()
        {
            Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetByEmailAsync("invalid-email"));
        }

        [Test]
        public void GetByEmailAsync_UserNotFound_Throws()
        {
            var email = "john@test.com";

            _repositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((User)null);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetByEmailAsync(email));
        }

        #endregion

        #region GetByUsernameAsync

        [Test]
        public async Task GetByUsernameAsync_UserExists_ReturnsUser()
        {
            var username = "john123";
            var user = new User { Username = username };

            _repositoryMock
                .Setup(r => r.GetByUsernameAsync(username))
                .ReturnsAsync(user);

            var result = await _service.GetByUsernameAsync(username);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
        }

        [Test]
        public void GetByUsernameAsync_NotFound_Throws()
        {
            var username = "john123";

            _repositoryMock
                .Setup(r => r.GetByUsernameAsync(username))
                .ReturnsAsync((User)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetByUsernameAsync(username));
        }

        #endregion

        #region UpdateAsync

        [Test]
        public async Task UpdateAsync_ValidUser_UpdatesSuccessfully()
        {
            var existingUser = new User
            {
                Name = "Old",
                Username = "john123",
                Email = "old@test.com"
            };

            var updatedUser = new User
            {
                Name = "New",
                Username = "john123",
                Email = "new@test.com"
            };

            _repositoryMock
                .Setup(r => r.GetByUsernameAsync(updatedUser.Username))
                .ReturnsAsync(existingUser);

            await _service.UpdateAsync(updatedUser);

            Assert.That(existingUser.Name, Is.EqualTo("New"));
            Assert.That(existingUser.Email, Is.EqualTo("new@test.com"));

            _repositoryMock.Verify(r => r.UpdateAsync(existingUser), Times.Once);
        }

        [Test]
        public void UpdateAsync_UserNotFound_Throws()
        {
            var user = new User { Username = "john123" };

            _repositoryMock
                .Setup(r => r.GetByUsernameAsync(user.Username))
                .ReturnsAsync((User)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateAsync(user));
        }

        #endregion

        #region DeleteAsync

        [Test]
        public async Task DeleteAsync_ValidUsername_CallsRepository()
        {
            var username = "john123";

            _repositoryMock
                .Setup(r => r.GetByUsernameAsync(username))
                .ReturnsAsync(new User { Username = username });

            await _service.DeleteAsync(username);

            _repositoryMock.Verify(r => r.DeleteAsync(username), Times.Once);
        }

        [Test]
        public void DeleteAsync_UserNotFound_Throws()
        {
            var username = "john123";

            _repositoryMock
                .Setup(r => r.GetByUsernameAsync(username))
                .ReturnsAsync((User)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteAsync(username));
        }

        #endregion

        #region GetManyAsync

        [Test]
        public async Task GetManyAsync_ReturnsUsers()
        {
            var users = new List<User>
            {
                new User { Username = "user1" },
                new User { Username = "user2" }
            };

            _repositoryMock
                .Setup(r => r.GetManyAsync())
                .ReturnsAsync(users);

            var result = await _service.GetManyAsync();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetManyAsync_Null_Throws()
        {
            _repositoryMock
                .Setup(r => r.GetManyAsync())
                .ReturnsAsync((IEnumerable<User>)null);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.GetManyAsync());
        }

        #endregion
    }
}