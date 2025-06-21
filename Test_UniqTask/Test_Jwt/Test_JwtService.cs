using ApplicationLayer.Services.JwtService;
using DomainLayer.Models;
using Microsoft.Extensions.Configuration;
using Xunit;
// Esta class es una unitaria para verificar que se genere un token JWT valido con usuario correcto

namespace Test_UniqTask.Test_Jwt
{
    public class Test_JwtService
    {
        [Fact]
        public void GenerateToken_UsuarioValido_TokenGenerado()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                { "Jwt:Key", "MiClaveSuperLargaYSegura12345678901234" },
                {"Jwt:Issuer", "TestIssuer"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var jwtService = new JwtService(config);

            var usuario = new Usuario
            {
                Username = "admin",
                Rol = "Administrador"
            };

            // Act
            var token = jwtService.GenerateToken(usuario);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
        }
    }
}