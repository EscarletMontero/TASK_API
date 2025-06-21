using ApplicationLayer.Services.Reactive;
using DomainLayer.Models;

// Claas tipo test uniq  para comprobar que el evento OnTaskCreated emite correctamente una tarea

namespace Test_UniqTask.Test_Reactive
{
    public class ReactiveTaskTests
    {
        [Fact]
        public void OnTaskCreated_EmiteEvento_RecibidoPorObservador()
        {
            // Arrange
            var reactive = new ReactiveTask();
            Tareas? tareaRecibida = null;
            var tarea = new Tareas { Description = "Tarea de prueba", DueDate = DateTime.Now.AddDays(1) };

            reactive.TaskStream.Subscribe(t => tareaRecibida = t);

            // Act
            reactive.OnTaskCreated(tarea);

            // Assert
            Assert.NotNull(tareaRecibida);
            Assert.Equal("Tarea de prueba", tareaRecibida.Description);
        }
    }
}
