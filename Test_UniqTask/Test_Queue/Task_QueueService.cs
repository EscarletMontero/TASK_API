using ApplicationLayer.Services.ColaServices;
using DomainLayer.Models;
using ApplicationLayer.Services.Reactive;

namespace Test_UniqTask.Test_Queue
{
    // Prueba uniq que es para verificar que TaskQueueService puede encolar y desencolar correctamente una tarea

    public class TaskQueueServiceTests
    {
        [Fact]
        public void EnqueueYDequeue_TareaPendiente_Exito()
        {
            // Arrange
            var reactive = new ReactiveTask();
            var queueService = new TaskQueueService(reactive);

            var tarea = new Tareas
            {
                Id = 1,
                Description = "Tarea desde cola",
                DueDate = DateTime.Now.AddDays(1),
                Status = "pendiente"
            };

            // Act 
            reactive.OnTaskCreated(tarea);

            // es un pequeño delay para que se procese el evento
            Task.Delay(100).Wait();

            // Assert
            Assert.True(queueService.TryDequeue(out var tareaDesencolada));
            Assert.Equal(tarea.Id, tareaDesencolada.Id);
            Assert.Equal("pendiente", tareaDesencolada.Status);
        }
    }
}

