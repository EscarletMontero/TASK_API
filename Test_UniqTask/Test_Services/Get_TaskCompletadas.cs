using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer.Models;
using Moq;
using Test_UniqTask.Test_Services.Base;
using Xunit;

namespace Test_UniqTask.Test_Services
{
    // Pruebas uniq que verifica que solo se retornen las tareas con estado completada

    public class GetCompletadasTests : TaskService_Base
    {
        [Fact]
        public async Task GetCompletadasAsync_DeberiaRetornarSoloCompletadas()
        {
            var lista = new List<Tareas>
            {
                new Tareas { Status = "pendiente" },
                new Tareas { Status = "completada" }
            };

            _mockCommons.Setup(r => r.GetAllAsync()).ReturnsAsync(lista);

            var result = await CreateService().GetCompletadasAsync();

            Assert.True(result.Successful);
            Assert.All(result.DataList, t => Assert.Equal("completada", t.Status));
        }
    }
}
