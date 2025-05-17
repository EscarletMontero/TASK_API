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
        public DateTime DueDate { get; set; }
        public string? Status { get; set; }
        public string? AdditionalData { get; set; }
    }
}