using Entities;
using Grpc.Core;
using GrpcRepositories;
using GrpcRepositories.Services;
using Moq;
using NUnit.Framework;
using RepositoryContracts;

namespace UnitTests
{
    [TestFixture]
    public class AuthRepositoryGrpcTests
    {
        private Mock<AuthServiceProto.AuthServiceProtoClient> _grpcClientMock;
        private IAuthRepository _repository;

        [SetUp]
        public void Setup()
        {
            _grpcClientMock = new Mock<AuthServiceProto.AuthServiceProtoClient>();
            _repository = new AuthRepositoryGrpc(_grpcClientMock.Object);
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsMappedUser()
        {
            // Arrange
            var user = new User
            {
                Name = "John Doe",
                Username = "johndoe",
                Password = "password123",
                Email = "john.doe@example.com"
            };

            var grpcResponse = new UserResponse
            {
                Name = user.Name,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email
            };

            _grpcClientMock
                .Setup(x => x.LoginAsync(
                    It.Is<AuthRequest>(r =>
                        r.Username == user.Username &&
                        r.Password == user.Password &&
                        r.Email == user.Email),
                    null, null, default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var result = await _repository.LoginAsync(user);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(user.Username));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Password, Is.EqualTo(user.Password));

            _grpcClientMock.Verify(x => x.LoginAsync(
                It.Is<AuthRequest>(r =>
                    r.Username == user.Username &&
                    r.Password == user.Password &&
                    r.Email == user.Email),
                null, null, default), Times.Once);
        }

        [Test]
        public void LoginAsync_UserNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = new User
            {
                Username = "johndoe",
                Password = "password123",
                Email = "john.doe@example.com"
            };

            _grpcClientMock
                .Setup(x => x.LoginAsync(
                    It.Is<AuthRequest>(r =>
                        r.Username == user.Username &&
                        r.Password == user.Password &&
                        r.Email == user.Email),
                    null, null, default))
                .Throws(new RpcException(new Status(StatusCode.NotFound, "User not found")));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _repository.LoginAsync(user));

            _grpcClientMock.Verify(x => x.LoginAsync(
                It.Is<AuthRequest>(r =>
                    r.Username == user.Username &&
                    r.Password == user.Password &&
                    r.Email == user.Email),
                null, null, default), Times.Once);
        }

        [Test]
        public void LoginAsync_InvalidOperation_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = new User
            {
                Username = "johndoe",
                Password = "password123",
                Email = "john.doe@example.com"
            };

            _grpcClientMock
                .Setup(x => x.LoginAsync(
                    It.Is<AuthRequest>(r =>
                        r.Username == user.Username &&
                        r.Password == user.Password &&
                        r.Email == user.Email),
                    null, null, default))
                .Throws(new InvalidOperationException("Invalid credentials"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _repository.LoginAsync(user));

            _grpcClientMock.Verify(x => x.LoginAsync(
                It.Is<AuthRequest>(r =>
                    r.Username == user.Username &&
                    r.Password == user.Password &&
                    r.Email == user.Email),
                null, null, default), Times.Once);
        }
    }
}