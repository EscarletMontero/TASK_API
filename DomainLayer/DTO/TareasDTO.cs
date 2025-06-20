using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.DTO
{
            // TareasDTO es  un objeto para transferir los datos del cliente y Controller
           public class TareasDTO
        {
            public string Description { get; set; }
            // No puse Status para que el usuario no los escriba ya que esta de forma predeterminada 
            public DateTime DueDate { get; set; }

            public string? AdditionalData { get; set; } 
       



      }
    }