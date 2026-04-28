using Entities.Plant;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcRepositories;
using GrpcRepositories.Services;
using Moq;

namespace UnitTests;

[TestFixture]
public class SoilHumidityGrpcTests
{
    private Mock<SoilHumidityServiceProto.SoilHumidityServiceProtoClient> _grpcMock;
    private SoilHumidityRepositoryGrpc _repository;

    [SetUp]
    public void Setup()
    {
        _grpcMock = new Mock<SoilHumidityServiceProto.SoilHumidityServiceProtoClient>();
        _repository = new SoilHumidityRepositoryGrpc(_grpcMock.Object);
    }

    [Test] 
    public async Task CreateAsync_ValidValue_CallsGrpc()
    {
        var humidity = new SoilHumidity { Value = 55.5 };

        _grpcMock.Setup(x => x.CreateAsync(
                It.IsAny<CreateSoilHumidityRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new Empty()));

        await _repository.CreateAsync("MAC1", humidity);

        _grpcMock.Verify(x => x.CreateAsync(
                It.Is<CreateSoilHumidityRequest>(r =>
                    r.PlantMAC == "MAC1" &&
                    r.Value == 55.5),
                null, null, default),
            Times.Once);
    }
    
    [Test]
    public async Task CreateAsync_NullValue_SendsZero()
    {
        var humidity = new SoilHumidity { Value = null };

        _grpcMock.Setup(x => x.CreateAsync(
                It.IsAny<CreateSoilHumidityRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new Empty()));

        await _repository.CreateAsync("MAC1", humidity);

        _grpcMock.Verify(x => x.CreateAsync(
                It.Is<CreateSoilHumidityRequest>(r =>
                    r.Value == 0.0),
                null, null, default),
            Times.Once);
    }
    
    [Test]
    public void CreateAsync_GrpcThrows_ThrowsInvalidOperationException()
    {
        var humidity = new SoilHumidity { Value = 30 };

        _grpcMock.Setup(x => x.CreateAsync(
                It.IsAny<CreateSoilHumidityRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.Internal, "error")));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _repository.CreateAsync("MAC1", humidity));
    }
    
    [Test]
    public void CreateAsync_UnexpectedException_ThrowsGenericException()
    {
        var humidity = new SoilHumidity { Value = 30 };

        _grpcMock.Setup(x => x.CreateAsync(
                It.IsAny<CreateSoilHumidityRequest>(), null, null, default))
            .Throws(new Exception("random failure"));

        Assert.ThrowsAsync<Exception>(async () =>
            await _repository.CreateAsync("MAC1", humidity));
    }
}