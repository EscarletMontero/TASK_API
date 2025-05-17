using DomainLayer.Models;
using InfrastructuraLayer.Context;
using InfrastructuraLayer.Repositorio.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructuraLayer.Repositorio.TaskRepository
{
    public class TaskRepository : ICommonsProccess<Tareas>
    {



        private readonly TaskApiContext _context;
        public TaskRepository(TaskApiContext taskApiContext)
        {
            _context = taskApiContext;
        }

        public async Task<IEnumerable<Tareas>> GetAllAsync()
            => await _context.Tarea.ToListAsync();

        public async Task<Tareas> GetByIdAsync(int id)
            => await _context.Tarea.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<(bool IsSuccess, string Message)> AddAsync(Tareas entry)
        {
            try
            {
                await _context.Tarea.AddAsync(entry);
                await _context.SaveChangesAsync();
                return (true, "La tarea se guardó correctamente...");
            }
            catch (Exception)
            {
                return (false, "No se pudo guardar la tarea...");
            }
        }

        public async Task<(bool IsSuccess, string Message)> UpdateAsync(Tareas entry)
        {
            try
            {
                _context.Tarea.Update(entry);
                await _context.SaveChangesAsync();
                return (true, "La tarea se actualizó correctamente...");
            }
            catch (Exception)
            {
                return (false, "No se pudo actualizar la tarea...");
            }
        }

        public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
        {
            try
            {
                var tarea = await _context.Tarea.FindAsync(id);
                if (tarea != null)
                {
                    _context.Tarea.Remove(tarea);
                    await _context.SaveChangesAsync();
                    return (true, "La tarea se eliminó correctamente...");
                }
                else
                {
                    return (false, "No se encontró la tarea...");
                }
            }
            catch (Exception)
            {
                return (false, "No se pudo eliminar la tarea...");
            }
        }

        public async Task<Tareas> GetIdAsync(int Id)
        => await _context.Tarea.FirstOrDefaultAsync(x => x.Id == Id);

    }
}
