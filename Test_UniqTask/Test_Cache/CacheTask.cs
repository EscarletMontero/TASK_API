using DomainLayer.Memory;
// Para verificar que MemorizacionCache almacena y recupera correctamente

namespace Test_UniqTask.Test_Memory
{
    public class CacheTests
    {
        [Fact]
        public void MemorizacionCache_GuardarYObtener_ValorCorrecto()
        {
            // Arrange
            var clave = MemorizacionCache<string, int>.GenerarClave("testCache");
            MemorizacionCache<string, int>.Limpiar();

            // Act
            MemorizacionCache<string, int>.Guardar(clave, 100);
            var resultado = MemorizacionCache<string, int>.Obtener(clave);

            // Assert
            Assert.Equal(100, resultado);
        }
    }
}
