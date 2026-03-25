using System.Linq;
using System.Threading.Tasks;
using Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcRepositories;
using GrpcRepositories.Services;
using Moq;
using NUnit.Framework;
using RepositoryContracts;

namespace UnitTests
{
    [TestFixture]
    public class UserRepositoryGrpcTests
    {
        private Mock<UserServiceProto.UserServiceProtoClient> _grpcClientMock;
        private IUserRepository _repository;

        [SetUp]
        public void Setup()
        {
            _grpcClientMock = new Mock<UserServiceProto.UserServiceProtoClient>();
            _repository = new UserRepositoryGrpc(_grpcClientMock.Object);
        }

        [Test]
        public async Task CreateAsync_ValidUser_ReturnsCreatedUser()
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
                .Setup(x => x.GetAsync(
                    It.Is<GetUserRequest>(r => r.Username == user.Username),
                    null,
                    null,
                    default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall<UserResponse>(null));
            
            _grpcClientMock
                .Setup(x => x.CreateAsync(
                    It.Is<CreateUserRequest>(r =>
                        r.Name == user.Name &&
                        r.Username == user.Username &&
                        r.Password == user.Password &&
                        r.Email == user.Email),
                    null,
                    null,
                    default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var result = await _repository.CreateAsync(user);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Username, Is.EqualTo(user.Username));
            Assert.That(result.Password, Is.EqualTo(user.Password));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        }

        [Test]
        public async Task GetByEmailAsync_UserExists_ReturnsMappedUser()
        {
            // Arrange
            var email = "john.doe@example.com";

            var grpcResponse = new UserResponse
            {
                Name = "John Doe",
                Username = "johndoe",
                Password = "password123",
                Email = email
            };

            _grpcClientMock
                .Setup(x => x.GetAsync(
                    It.Is<GetUserRequest>(r => r.Email == email),
                    null,
                    null,
                    default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var result = await _repository.GetByEmailAsync(email);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(email));
            Assert.That(result.Username, Is.EqualTo("johndoe"));
        }

        [Test]
        public async Task GetByUsernameAsync_UserExists_ReturnsMappedUser()
        {
            // Arrange
            var username = "johndoe";

            var grpcResponse = new UserResponse
            {
                Name = "John Doe",
                Username = username,
                Password = "password123",
                Email = "john.doe@example.com"
            };

            _grpcClientMock
                .Setup(x => x.GetAsync(
                    It.Is<GetUserRequest>(r => r.Username == username),
                    null,
                    null,
                    default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(grpcResponse));

            // Act
            var result = await _repository.GetByUsernameAsync(username);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
            Assert.That(result.Email, Is.EqualTo("john.doe@example.com"));
        }

        [Test]
        public async Task DeleteAsync_UsernameProvided_CallsGrpcDelete()
        {
            // Arrange
            var username = "johndoe";

            _grpcClientMock
                .Setup(x => x.DeleteAsync(
                    It.Is<DeleteUserRequest>(r => r.Username == username),
                    null,
                    null,
                    default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new Empty()));

            // Act
            await _repository.DeleteAsync(username);

            // Assert
            _grpcClientMock.Verify(x => x.DeleteAsync(
                It.Is<DeleteUserRequest>(r => r.Username == username),
                null,
                null,
                default), Times.Once);
        }

        [Test]
        public async Task GetManyAsync_ReturnsUsers()
        {
            // Arrange
            var grpcResponse = new GetManyUserResponse();
            grpcResponse.Users.Add(new UserResponse
            {
                Name = "John Doe",
                Username = "johndoe",
                Password = "password123",
                Email = "john.doe@example.com"
            });
            grpcResponse.Users.Add(new UserResponse
            {
                Name = "Jane Doe",
                Username = "janedoe",
                Password = "password456",
                Email = "jane.doe@example.com"
            });

            _grpcClientMock
                .Setup(x => x.GetAllAsync(
                    It.IsAny<Google.Protobuf.WellKnownTypes.Empty>(),
                    null,
                    null,
                    default))
                .Returns(new AsyncUnaryCall<GetManyUserResponse>(
                    Task.FromResult(grpcResponse),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
                ));

            // Act
            var result = await _repository.GetManyAsync();

            // Assert
            Assert.That(result, Is.Not.Null);

            var users = result.ToList();
            Assert.That(users.Count, Is.EqualTo(2));
            Assert.That(users[0].Username, Is.EqualTo("johndoe"));
            Assert.That(users[1].Username, Is.EqualTo("janedoe"));
        }

        [Test]
        public async Task UpdateAsync_ValidUser_CallsGrpcUpdate()
        {
            // Arrange
            var user = new User
            {
                Name = "John Doe Updated",
                Username = "johndoe",
                Password = "newpassword123",
                Email = "john.doe@example.com"
            };

            var existingUserResponse = new UserResponse
            {
                Name = "John Doe",
                Username = "johndoe",
                Password = "oldpassword",
                Email = "john.doe@example.com"
            };

            _grpcClientMock
                .Setup(x => x.GetAsync(
                    It.Is<GetUserRequest>(r => r.Username == user.Username),
                    null,
                    null,
                    default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(existingUserResponse));

            _grpcClientMock
                .Setup(x => x.UpdateAsync(
                    It.Is<UpdateUserRequest>(r =>
                        r.Name == user.Name &&
                        r.Username == user.Username &&
                        r.Password == user.Password &&
                        r.Email == user.Email),
                    null,
                    null,
                    default))
                .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new Empty()));

            // Act
            await _repository.UpdateAsync(user);

            // Assert
            _grpcClientMock.Verify(x => x.GetAsync(
                It.Is<GetUserRequest>(r => r.Username == user.Username),
                null,
                null,
                default), Times.Once);

            _grpcClientMock.Verify(x => x.UpdateAsync(
                It.Is<UpdateUserRequest>(r =>
                    r.Name == user.Name &&
                    r.Username == user.Username &&
                    r.Password == user.Password &&
                    r.Email == user.Email),
                null,
                null,
                default), Times.Once);
        }
        public void Test_CreateAsync_UserExists()
{
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
        .Setup(x => x.GetAsync(
            It.Is<GetUserRequest>(r => r.Username == user.Username),
            null,
            null,
            default))
        .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(grpcResponse));

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.CreateAsync(user));

    _grpcClientMock.Verify(x => x.CreateAsync(
        It.IsAny<CreateUserRequest>(),
        null,
        null,
        default), Times.Never);
}

[Test]
public void Test_GetByEmailAsync_UserMissing()
{
    var email = "john.doe@example.com";

    _grpcClientMock
        .Setup(x => x.GetAsync(
            It.Is<GetUserRequest>(r => r.Email == email),
            null,
            null,
            default))
        .Returns(GrpcMockHelpers.CreateAsyncUnaryCall<UserResponse>(null));

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.GetByEmailAsync(email));
}

[Test]
public void Test_GetByEmailAsync_UserLookupThrows()
{
    var email = "john.doe@example.com";

    _grpcClientMock
        .Setup(x => x.GetAsync(
            It.Is<GetUserRequest>(r => r.Email == email),
            null,
            null,
            default))
        .Throws(new RpcException(new Status(StatusCode.NotFound, "User not found")));

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.GetByEmailAsync(email));
}

[Test]
public void Test_GetByUsernameAsync_UserMissing()
{
    var username = "johndoe";

    _grpcClientMock
        .Setup(x => x.GetAsync(
            It.Is<GetUserRequest>(r => r.Username == username),
            null,
            null,
            default))
        .Returns(GrpcMockHelpers.CreateAsyncUnaryCall<UserResponse>(null));

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.GetByUsernameAsync(username));
}

[Test]
public void Test_GetByUsernameAsync_UserLookupThrows()
{
    var username = "johndoe";

    _grpcClientMock
        .Setup(x => x.GetAsync(
            It.Is<GetUserRequest>(r => r.Username == username),
            null,
            null,
            default))
        .Throws(new RpcException(new Status(StatusCode.NotFound, "User not found")));

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.GetByUsernameAsync(username));
}

[Test]
public void Test_UpdateAsync_UserMissing()
{
    var user = new User
    {
        Name = "John Doe Updated",
        Username = "johndoe",
        Password = "newpassword123",
        Email = "john.doe@example.com"
    };

    _grpcClientMock
        .Setup(x => x.GetAsync(
            It.Is<GetUserRequest>(r => r.Username == user.Username),
            null,
            null,
            default))
        .Returns(GrpcMockHelpers.CreateAsyncUnaryCall<UserResponse>(null));

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.UpdateAsync(user));

    _grpcClientMock.Verify(x => x.UpdateAsync(
        It.IsAny<UpdateUserRequest>(),
        null,
        null,
        default), Times.Never);
}

[Test]
public void Test_UpdateAsync_UserLookupThrows()
{
    var user = new User
    {
        Name = "John Doe Updated",
        Username = "johndoe",
        Password = "newpassword123",
        Email = "john.doe@example.com"
    };

    _grpcClientMock
        .Setup(x => x.GetAsync(
            It.Is<GetUserRequest>(r => r.Username == user.Username),
            null,
            null,
            default))
        .Throws(new RpcException(new Status(StatusCode.NotFound, "User not found")));

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
        await _repository.UpdateAsync(user));

    _grpcClientMock.Verify(x => x.UpdateAsync(
        It.IsAny<UpdateUserRequest>(),
        null,
        null,
        default), Times.Never);
}
    }
    
    public static class GrpcMockHelpers
    {
        public static AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response)
        {
            return new AsyncUnaryCall<T>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }
    }
    
}