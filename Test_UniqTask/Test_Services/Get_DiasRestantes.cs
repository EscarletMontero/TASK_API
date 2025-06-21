using System;
using System.Threading.Tasks;
using DomainLayer.Models;
using Moq;
using Test_UniqTask.Test_Services.Base;
using Xunit;

namespace Test_UniqTask.Test_Services
{
      // Pruebas verifica el calculo de dias restantes hasta la fecha de vencimiento de una tarea


    public class GetDiasRestantesTests : TaskService_Base
    {
        [Fact]
        public async Task GetDiasRestantesAsync_DeberiaCalcularCorrectamente()
        {
            var tarea = new Tareas
            {
                Id = 1,
                DueDate = DateTime.Now.AddDays(3),
                Description = "Prueba"
            };

            _mockCommons.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tarea);

            var result = await CreateService().GetDiasRestantesAsync(1);

            Assert.True(result.Successful);
            Assert.Contains("Faltan", result.Message);
        }
    }
}
