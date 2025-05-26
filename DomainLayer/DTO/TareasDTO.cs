using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTO
{
    // TareasDTO es  un objeto para transferir  datos para comunicar el cliente y la API Controller
           public class TareasDTO
        {
            public string Description { get; set; }
            // No puse DueDate y Status para que el usuario no los envie
            public string? AdditionalData { get; set; } // Esto puede ser nulo si no se proporciona
        }
    }