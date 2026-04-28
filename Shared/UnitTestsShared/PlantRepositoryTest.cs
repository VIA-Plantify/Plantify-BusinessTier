using System.Linq;
using System.Threading.Tasks;
using Entities.Plant;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcRepositories;
using GrpcRepositories.Services;
using Moq;
using NUnit.Framework;
using RepositoryContracts;

namespace UnitTests;

[TestFixture]
public class PlantRepositoryGrpcTests
{
    private Mock<PlantServiceProto.PlantServiceProtoClient> _grpcMock;
    private IPlantRepository _repository;

    [SetUp]
    public void Setup()
    {
        _grpcMock = new Mock<PlantServiceProto.PlantServiceProtoClient>();
        _repository = new PlantRepositoryGrpc(_grpcMock.Object);
    }

    [Test]
    public async Task CreateAsync_PlantDoesNotExist_CreatesPlant()
    {
        var plant = new Plant { MAC = "MAC1", Name = "Rose" };

        // First call: GetPlantAsync -> NOT FOUND
        _grpcMock.Setup(x => x.GetAsync(
                It.IsAny<GetPlantRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.NotFound, "not found")));

        // Second call: CreateAsync -> success
        _grpcMock.Setup(x => x.CreateAsync(
                It.IsAny<CreatePlantRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new PlantResponse
            {
                PlantMAC = "MAC1",
                Name = "Rose"
            }));

        var result = await _repository.CreateAsync("user1", plant);

        Assert.That(result.MAC, Is.EqualTo("MAC1"));
    }
    
    [Test]
    public void CreateAsync_PlantExists_Throws()
    {
        _grpcMock.Setup(x => x.GetAsync(
                It.IsAny<GetPlantRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new PlantResponse
            {
                PlantMAC = "MAC1"
            }));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _repository.CreateAsync("user1", new Plant { MAC = "MAC1" }));
    }
    
    [Test]
    public async Task GetPlantsByUsername_ReturnsPlants()
    {
        var response = new GetManyPlantResponse();
        response.Plants.Add(new PlantResponse { PlantMAC = "MAC1", Name = "Rose" });

        _grpcMock.Setup(x => x.GetPlantsByUsernameAsync(
                It.IsAny<GetPlantsByUsernameRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(response));

        var result = (await _repository.GetPlantsByUsernameAsync("user1")).ToList();

        Assert.That(result.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void GetPlantsByUsername_NotFound_Throws()
    {
        _grpcMock.Setup(x => x.GetPlantsByUsernameAsync(
                It.IsAny<GetPlantsByUsernameRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.NotFound, "not found")));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _repository.GetPlantsByUsernameAsync("user1"));
    }
    
    [Test]
    public async Task GetPlantAsync_Found_ReturnsPlant()
    {
        _grpcMock.Setup(x => x.GetAsync(
                It.IsAny<GetPlantRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new PlantResponse
            {
                PlantMAC = "MAC1",
                Name = "Rose"
            }));

        var result = await _repository.GetPlantAsync("user1", "MAC1");

        Assert.That(result.MAC, Is.EqualTo("MAC1"));
    }
    
    [Test]
    public void GetPlantAsync_NotFound_Throws()
    {
        _grpcMock.Setup(x => x.GetAsync(
                It.IsAny<GetPlantRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.NotFound, "not found")));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _repository.GetPlantAsync("user1", "MAC1"));
    }
    
    [Test] 
    public async Task DeleteAsync_Valid_CallsGrpc()
    {
        _grpcMock.Setup(x => x.DeleteAsync(
                It.IsAny<DeletePlantRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new Empty()));

        await _repository.DeleteAsync("user1", "MAC1");

        _grpcMock.Verify(x => x.DeleteAsync(
            It.Is<DeletePlantRequest>(r => r.PlantMAC == "MAC1"),
            null, null, default), Times.Once);
    }
    
    [Test]
    public void DeleteAsync_NotFound_Throws()
    {
        _grpcMock.Setup(x => x.DeleteAsync(
                It.IsAny<DeletePlantRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.NotFound, "not found")));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _repository.DeleteAsync("user1", "MAC1"));
    }
    
    [Test]
    public async Task UpdateAsync_Valid_CallsGrpc()
    {
        var plant = new Plant { MAC = "MAC1", Name = "Updated" };

        _grpcMock.Setup(x => x.UpdateAsync(
                It.IsAny<UpdatePlantRequest>(), null, null, default))
            .Returns(GrpcMockHelpers.CreateAsyncUnaryCall(new Empty()));

        await _repository.UpdateAsync("user1", plant);

        _grpcMock.Verify(x => x.UpdateAsync(
            It.Is<UpdatePlantRequest>(r => r.PlantMAC == "MAC1"),
            null, null, default), Times.Once);
    }
    
    [Test]
    public void UpdateAsync_NotFound_Throws()
    {
        var plant = new Plant { MAC = "MAC1" };

        _grpcMock.Setup(x => x.UpdateAsync(
                It.IsAny<UpdatePlantRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.NotFound, "not found")));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _repository.UpdateAsync("user1", plant));
    }
    
    
}