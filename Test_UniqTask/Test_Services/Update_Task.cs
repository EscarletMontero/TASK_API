using DomainLayer.Models;
using Moq;
using Test_UniqTask.Test_Services.Base;

namespace Test_UniqTask.Test_Services
{
    // Pruebas uniq para el metodo UpdateAsync del servicio TaskServices
    // Para validar que las tareas se actualizan de forma correcta 
    public class Update_Task : TaskService_Base
    {
        [Fact]
        public async Task UpdateAsync_SuccessfulUpdate()
        {
            var tarea = new Tareas
            {
                Id = 1,
                Description = "Update",
                Status = "pendiente",
                DueDate = DateTime.Now.AddDays(1)
            };

            _mockCommons.Setup(c => c.UpdateAsync(tarea)).ReturnsAsync((true, "Actualizada"));

            var result = await CreateService().UpdateAsync(tarea);

            Assert.True(result.Successful);
            Assert.Equal("Actualizada", result.Message);
        }
    }
}