using DomainLayer.Models;
using DomainLayer.Delegates;
// Prueba uniq para validar que una tarea con descripcion vacia o fecha pasada sea invalida x

namespace Test_UniqTask.Test_Delegates
{
    public class ValidarTarea
    {
        [Fact]
        public void ValidarTarea_TareaInvalida_RetornaFalse()
        {
            // Arrange
            var tarea = new Tareas
            {
                Description = "",
                DueDate = DateTime.Now.AddDays(-1) // Fecha pasada 
            };

            // Act
            var resultado = Delegates.ValidarTarea(tarea);

            // Assert
            Assert.False(resultado);
        }
    }
}
