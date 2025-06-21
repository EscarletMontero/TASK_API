using ApplicationLayer.Services.ColaServices;
using ApplicationLayer.Services.Reactive;
using ApplicationLayer.Services.TaskServices;
using DomainLayer.Models;
using InfrastructuraLayer.Repositorio.Commons;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Proyect_TaskAPI.Hubs;

namespace Test_UniqTask.Test_Services.Base
{// Clase base abstracta para las pruebas de TaskServices, la uso para centraliza los mocks y la creacion del servicio
 // y asi evitar duplicar codigo.


    public abstract class TaskService_Base
    {
        protected readonly Mock<ICommonsProccess<Tareas>> _mockCommons;
        protected readonly Mock<ITaskQueueService> _mockQueue;
        protected readonly Mock<IHubContext<TareaHub>> _mockHub;
        protected readonly ReactiveTask _reactiveTask;

        protected TaskService_Base()
        {
            _mockCommons = new Mock<ICommonsProccess<Tareas>>();
            _mockQueue = new Mock<ITaskQueueService>();
            _mockHub = new Mock<IHubContext<TareaHub>>();
            _reactiveTask = new ReactiveTask();

            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            _mockHub.Setup(h => h.Clients).Returns(mockClients.Object);
        }

        protected TaskServices CreateService() =>
            new TaskServices(_mockCommons.Object, _mockQueue.Object, _reactiveTask, _mockHub.Object);
    }
}
