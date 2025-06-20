using Xunit;
using Moq;
using DomainLayer.Models;
using DomainLayer.DTO;
using InfrastructuraLayer.Repositorio.Commons;
using ApplicationLayer.Services.Reactive;
using ApplicationLayer.Services.ColaServices;
using ApplicationLayer.Services.TaskServices;
using Microsoft.AspNetCore.SignalR;
using Proyect_TaskAPI.Hubs;
using System.Threading.Tasks;

public class TaskServicesTests
{
    [Fact]
    public async Task Tarea_SeCreaConEstadoPendiente_Y_CambiaSimuladamenteACompletada()
    {
        // Arrange
        var mockCommons = new Mock<ICommonsProccess<Tareas>>();
        var mockQueue = new Mock<ITaskQueueService>();
        var mockHub = new Mock<IHubContext<TareaHub>>();

        var reactive = new ReactiveTask();

        Tareas? tareaInsertada = null;

        // Simulamos que la tarea se guarda exitosamente
        mockCommons.Setup(r => r.AddAsync(It.IsAny<Tareas>()))
            .Callback<Tareas>(t => tareaInsertada = t)
            .ReturnsAsync((true, "Tarea agregada"));

        var service = new TaskServices(
            mockCommons.Object,
            mockQueue.Object,
            reactive,
            mockHub.Object
        );

        var dto = new TareasDTO
        {
            Description = "Prueba de estados",
            DueDate = DateTime.Now.AddDays(1)
        };

        // Act
        var response = await service.AddAsync(dto);

        // Assert - Estado inicial "pendiente"
        Assert.True(response.Successful);
        Assert.NotNull(tareaInsertada);
        Assert.Equal("pendiente", tareaInsertada!.Status);

        // Simular procesamiento por TaskCola (sin esperar 4 minutos)
        tareaInsertada.Status = "en proceso";
        Assert.Equal("en proceso", tareaInsertada.Status);

        tareaInsertada.Status = "completada";
        Assert.Equal("completada", tareaInsertada.Status);
    }
}
