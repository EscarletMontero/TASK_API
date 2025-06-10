using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructuraLayer.Context
{
    public class TaskApiContext: DbContext
    {
        public TaskApiContext(DbContextOptions options): base(options)
        {
            
        }

        public DbSet<Tareas> Tarea { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tareas>()
                .Property(t => t.Status)
                .HasDefaultValue("pendiente");
        }




    }
}
