using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace DomainLayer.Models
    {
        public class Tareas
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }
           
            [Required]
            public string Description { get; set; }

            public DateTime DueDate { get; set; }

            public string Status { get; set; } = "pendiente"; 

            public string AdditionalData { get; set; }
        }
    }
