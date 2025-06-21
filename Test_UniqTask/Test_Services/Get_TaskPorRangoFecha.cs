using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer.Models;
using Moq;
using Test_UniqTask.Test_Services.Base;
using Xunit;

namespace Test_UniqTask.Test_Services
{
    // Pruebas uniq para asegura que el filtrado por rango de fechas funcione correctamente

    public class GetPorRangoFechaTests : TaskService_Base
    {
        [Fact]
        public async Task GetPorRangoFechaAsync_DeberiaFiltrarCorrectamente()
        {
            var lista = new List<Tareas>
            {
                new Tareas { DueDate = DateTime.Now.Date },
                new Tareas { DueDate = DateTime.Now.AddDays(5).Date }
            };

            _mockCommons.Setup(r => r.GetAllAsync()).ReturnsAsync(lista);

            var desde = DateTime.Now.Date;
            var hasta = DateTime.Now.AddDays(3).Date;

            var result = await CreateService().GetPorRangoFechaAsync(desde, hasta);

            Assert.True(result.Successful);
            Assert.All(result.DataList, t => Assert.InRange(t.DueDate, desde, hasta));
        }
    }
}
