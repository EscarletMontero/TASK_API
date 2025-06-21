using DomainLayer.Models;
using DomainLayer.Delegates;
// Prueba uniq para validar que Delegates.FiltrarTareasPendientes devuelva solo tareas pendientes

namespace Test_UniqTask.Test_Delegates
{
    public class FiltrarPendientesTests
    {
        [Fact]
        public void FiltrarTareasPendientes_DevuelveSoloPendientes()
        {
            // Arrange
            var lista = new List<Tareas>
            {
                new Tareas { Description = "1", Status = "pendiente" },
                new Tareas { Description = "2", Status = "completada" },
                new Tareas { Description = "3", Status = "pendiente" }
            };

            // Act
            var resultado = Delegates.FiltrarTareasPendientes(lista);

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.All(resultado, t => Assert.Equal("pendiente", t.Status));
        }
    }
}
