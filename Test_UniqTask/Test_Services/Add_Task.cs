using ApplicationLayer.Services.ColaServices;
using ApplicationLayer.Services.Reactive;
using ApplicationLayer.Services.TaskServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using InfrastructuraLayer.Repositorio.Commons;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Proyect_TaskAPI.Hubs;
using Test_UniqTask.Test_Services.Base;
namespace Test_UniqTask.Test_Services
{// Pruebas unitq para el metodo AddAsync del servicio TaskServices
 // Para verifica que las tareas se agregan correctamente y se encolan si estan pendientes.

    public class Add_Task :TaskService_Base
    {
        [Fact]
        public async Task AddAsync_DeberiaAgregarYEncolarSiEsPendiente()
        {
            // Arrange
            var service = CreateService();
            var dto = new TareasDTO { Description = "Test", DueDate = DateTime.Now.AddDays(1) };
            _mockCommons.Setup(x => x.AddAsync(It.IsAny<DomainLayer.Models.Tareas>()))
                        .ReturnsAsync((true, "OK"));

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            Assert.True(result.Successful);
            _mockQueue.Verify(q => q.Enqueue(It.IsAny<DomainLayer.Models.Tareas>()), Times.Once);
        }
    }

   }
     