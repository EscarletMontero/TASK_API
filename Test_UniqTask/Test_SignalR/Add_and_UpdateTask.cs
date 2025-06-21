using ApplicationLayer.Services.ColaServices;
using ApplicationLayer.Services.Reactive;
using ApplicationLayer.Services.TaskServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using InfrastructuraLayer.Repositorio.Commons;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Proyect_TaskAPI.Hubs;

namespace Test_UniqTask.Test_SignalR;

// Esta clase es para verificar que SignalR notifica a los clientes cuando se agrega o actualiza una tarea.
public class SignalR_TaskService_Notify
{
    private readonly Mock<ICommonsProccess<Tareas>> _mockCommons;
    private readonly Mock<ITaskQueueService> _mockQueue;
    private readonly ReactiveTask _reactiveTask;
    private readonly Mock<IHubContext<TareaHub>> _mockHub;
    private readonly TaskServices _taskService;

    public SignalR_TaskService_Notify()
    {
        _mockCommons = new Mock<ICommonsProccess<Tareas>>();
        _mockQueue = new Mock<ITaskQueueService>();
        _reactiveTask = new ReactiveTask();
        _mockHub = new Mock<IHubContext<TareaHub>>();

        // Simula los clientes de SignalR
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
        _mockHub.Setup(h => h.Clients).Returns(mockClients.Object);

        _taskService = new TaskServices(
            _mockCommons.Object,
            _mockQueue.Object,
            _reactiveTask,
            _mockHub.Object
        );
    }

    [Fact]
    public async Task AddAsync_EnviaNotificacionSignalR()
    {
        // Arrange
        var dto = new TareasDTO { Description = "Con SignalR", DueDate = DateTime.Now.AddDays(1) };
        _mockCommons.Setup(c => c.AddAsync(It.IsAny<Tareas>()))
                    .ReturnsAsync((true, "Tarea agregada"));

        // Act
        var result = await _taskService.AddAsync(dto);

        // Assert
        Assert.True(result.Successful);
        _mockHub.Verify(h => h.Clients.All.SendAsync("TareaCreada", It.IsAny<Tareas>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_EnviaNotificacionSignalR()
    {
        // Arrange
        var tarea = new Tareas { Id = 1, Description = "Actualizar", Status = "pendiente", DueDate = DateTime.Now.AddDays(1) };
        _mockCommons.Setup(c => c.UpdateAsync(tarea)).ReturnsAsync((true, "Actualizada"));

        // Act
        var result = await _taskService.UpdateAsync(tarea);

        // Assert
        Assert.True(result.Successful);
        _mockHub.Verify(h => h.Clients.All.SendAsync("TareaActualizada", tarea, default), Times.Once);
    }
}
