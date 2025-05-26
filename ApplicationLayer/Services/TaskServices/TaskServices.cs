using DomainLayer.Models;
using DomainLayer.Delegates;
using DomainLayer.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfrastructuraLayer.Repositorio.Commons;
using DomainLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using TaskFactory = DomainLayer.Factory.TaskFactory;

namespace ApplicationLayer.Services.TaskServices
{
    public class TaskService
    {
        private readonly ICommonsProccess<Tareas> _commonsProcess;

        public TaskService(ICommonsProccess<Tareas> commonsProcess)
        {
            _commonsProcess = commonsProcess;
        }

        /// <summary>
        /// Obtiene todas las tareas.
        /// </summary>
        public async Task<Response<Tareas>> GetAllAsync()
        {
            var response = new Response<Tareas>();
            try
            {
                var allTasks = await _commonsProcess.GetAllAsync();
                response.DataList = (List<Tareas>?)allTasks;
                response.Successful = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        /// <summary>
        /// Obtiene una tarea por su Id.
        /// </summary>
        public async Task<Response<Tareas>> GetByIdAsync(int id)
        {
            var response = new Response<Tareas>();
            try
            {
                var result = await _commonsProcess.GetIdAsync(id);
                if (result != null)
                {
                    response.SingleData = result;
                    response.Successful = true;
                }
                else
                {
                    response.Successful = false;
                    response.Message = "No se encontro informacion...";
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        /// <summary>
        /// Agrega una nueva tarea.
        /// </summary>
        public async Task<Response<Tareas>> AddAsync(TareasDTO dto)
        {
            var response = new Response<Tareas>();
            try
            {
                var tarea = new Tareas
                {
                    Description = dto.Description,
                    AdditionalData = dto.AdditionalData
                };

                if (!Delegates.ValidarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es válida. Verifica la descripción.";
                    return response;
                }

                var result = await _commonsProcess.AddAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (response.Successful)
                {
                    response.SingleData = tarea;
                    Delegates.NotificarEvento($"Tarea agregada: {tarea.Description}");
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }
            return response;
        }


        /// <summary>
        /// Actualiza una tarea existente.
        /// </summary>
        public async Task<Response<Tareas>> UpdateAsync(Tareas tarea)
        {
            var response = new Response<Tareas>();
            try
            {
                var result = await _commonsProcess.UpdateAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;
                // Ya no intentamos acceder a UpdatedEntity aqui, ya que la interfaz no lo define.
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }


        /// <summary>
        /// Elimina una tarea por su Id.
        /// </summary>
        public async Task<Response<Tareas>> DeleteAsync(int id)
        {
            var response = new Response<Tareas>();
            try
            {
                var tareaToDelete = await _commonsProcess.GetIdAsync(id);
                if (tareaToDelete == null)
                {
                    response.Successful = false;
                    response.Message = "La tarea no existe.";
                    return response;
                }

                var result = await _commonsProcess.DeleteAsync(id);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;
                if (response.Successful)
                {
                    response.SingleData = tareaToDelete; // Devolvemos la tarea eliminada para informacion
                    Delegates.NotificarEvento($"Tarea eliminada: {tareaToDelete.Description}");
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response; 
        }

        // LINQ y Delegados

        /// <summary>
        /// Obtiene las tareas pendientes.
        /// </summary>
        public async Task<Response<Tareas>> GetPendientesAsync()
        {
            var response = new Response<Tareas>();
            try
            {
                var all = await _commonsProcess.GetAllAsync();
                response.DataList = Delegates.FiltrarTareasPendientes(all);
                response.Successful = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        /// <summary>
        /// Obtiene las tareas completadas.
        /// </summary>
        public async Task<Response<Tareas>> GetCompletadasAsync()
        {
            var response = new Response<Tareas>();
            try
            {
                var all = await _commonsProcess.GetAllAsync();
                response.DataList = Delegates.FiltrarTareasCompletadas(all.ToList());
                response.Successful = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        /// <summary>
        /// Obtiene las tareas vencidas.
        /// </summary>
        public async Task<Response<Tareas>> GetVencidasAsync()
        {
            var response = new Response<Tareas>();
            try
            {
                var all = await _commonsProcess.GetAllAsync();
                response.DataList = Delegates.FiltrarTareasVencidas(all.ToList());
                response.Successful = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        /// <summary>
        /// Obtiene las tareas por rango de fecha de vencimiento.
        /// </summary>
        public async Task<Response<Tareas>> GetPorRangoFechaAsync(DateTime desde, DateTime hasta)
        {
            var response = new Response<Tareas>();
            try
            {
                var all = await _commonsProcess.GetAllAsync();
                response.DataList = all
                    .Where(t => t.DueDate.Date >= desde.Date && t.DueDate.Date <= hasta.Date)
                    .ToList();
                response.Successful = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        /// <summary>
        /// Obtiene los dias restantes para una tarea.
        /// </summary>
        public async Task<Response<string>> GetDiasRestantesAsync(int tareaId)
        {
            var response = new Response<string>();
            try
            {
                var tarea = await _commonsProcess.GetByIdAsync(tareaId);
                if (tarea == null)
                {
                    response.Successful = false;
                    response.Message = "Tarea no encontrada.";
                    return response;
                }

                int dias = Delegates.CalcularDiasRestantes(tarea.DueDate);
                response.Successful = true;
                response.Message = $"Faltan {dias} dias para la tarea: {tarea.Description}";
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

    }
}