using DomainLayer.Models;
using DomainLayer.Delegates;
using DomainLayer.Memory;
using InfrastructuraLayer.Repositorio.Commons;
using DomainLayer.DTO;
using Microsoft.AspNetCore.Mvc;
using TaskFactory = DomainLayer.Factory.TaskFactory;
using ApplicationLayer.Services.Reactive;

namespace ApplicationLayer.Services.TaskServices
{
    public class TaskService
    {
        // Dependencias inyectadas para el manejo común de procesos, cola de tareas y eventos reactivos.
        private readonly ICommonsProccess<Tareas> _commonsProcess;
        private readonly ITaskQueueService _queueService;
        private readonly ReactiveTask _reactiveTask;

        // Constructor que inicializa las dependencias.
        public TaskService(
            ICommonsProccess<Tareas> commonsProcess,
            ITaskQueueService queueService,
            ReactiveTask reactiveTask)
        {
            _commonsProcess = commonsProcess;
            _queueService = queueService;
            _reactiveTask = reactiveTask;
        }
        /// <summary>
        /// Obtiene una tarea por su ID.
        /// </summary>
        /// <param name="id">El ID de la tarea.</param>
        /// <returns>Respuesta con la tarea encontrada o mensaje de error.</returns>
        public async Task<Response<Tareas>> GetByIdAsync(int id)
        {
            var response = new Response<Tareas>();

            try
            {
                var tarea = await _commonsProcess.GetIdAsync(id);
                if (tarea == null)
                {
                    response.Successful = false;
                    response.Message = "Tarea no encontrada.";
                }
                else
                {
                    response.Successful = true;
                    response.SingleData = tarea;
                    response.Message = "Tarea obtenida exitosamente.";
                }
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Obtiene todas las tareas almacenadas.
        /// </summary>
        /// <returns>Respuesta con la lista de tareas o mensaje de error.</returns>
        public async Task<Response<Tareas>> GetAllAsync()
        {
            var response = new Response<Tareas>();

            try
            {
                var result = await _commonsProcess.GetAllAsync();  // IEnumerable<Tareas>
                response.Successful = true;
                response.DataList = result.ToList();  // List<Tareas>
                response.Message = "Lista de tareas obtenida correctamente.";
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Errors.Add(ex.Message);
            }

            return response;
        }



        /// <summary>
        /// Agrega una nueva tarea de forma asíncrona.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos de la tarea.</param>
        /// <returns>Objeto de respuesta con el resultado de la operación.</returns>
        public async Task<Response<Tareas>> AddAsync(TareasDTO dto)
        {
            var response = new Response<Tareas>();

            try
            {
                // Crea una nueva instancia de 'Tareas' a partir del DTO.
                var tarea = new Tareas
                {
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    Status = "pendiente", // Establece el estado inicial como "pendiente".
                    AdditionalData = dto.AdditionalData ?? string.Empty // Asigna datos adicionales o una cadena vacía.
                };

                // Valida la tarea antes de agregarla.
                if (!Delegates.ValidarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es valida. Verifica la descripcion y la fecha.";
                    return response;
                }

                // Intenta agregar la tarea usando el proceso común.
                var result = await _commonsProcess.AddAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                // Si la operación fue exitosa:
                if (response.Successful)
                {
                    response.SingleData = tarea; // Asigna la tarea agregada a la respuesta.
                    Delegates.NotificarEvento($"Tarea agregada: {tarea.Description}"); // Notifica un evento.

                    _reactiveTask.OnTaskCreated(tarea); // Emite un evento reactivo de creación de tarea.

                    // Si el estado es "pendiente", encola la tarea para procesamiento.
                    if (tarea.Status?.ToLowerInvariant() == "pendiente")
                        _queueService.Enqueue(tarea);
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción y agrega el mensaje de error a la respuesta.
                response.Errors.Add(ex.Message);
            }

            return response; // Devuelve la respuesta.
        }

        /// <summary>
        /// Actualiza una tarea existente de forma asíncrona.
        /// </summary>
        /// <param name="tarea">Objeto de la tarea a actualizar.</param>
        /// <returns>Objeto de respuesta con el resultado de la operación.</returns>
        public async Task<Response<Tareas>> UpdateAsync(Tareas tarea)
        {
            var response = new Response<Tareas>();

            try
            {
                // Intenta actualizar la tarea usando el proceso común.
                var result = await _commonsProcess.UpdateAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                // Si la operación fue exitosa:
                if (response.Successful)
                {
                    _reactiveTask.OnTaskUpdated(tarea); // Emite un evento reactivo de actualización de tarea.

                    // Si el estado es "pendiente", encola la tarea.
                    if (tarea.Status?.ToLowerInvariant() == "pendiente")
                        _queueService.Enqueue(tarea);
                }
            }
            catch (Exception e)
            {
                // Captura cualquier excepción y agrega el mensaje de error a la respuesta.
                response.Errors.Add(e.Message);
            }

            return response; // Devuelve la respuesta.
        }

        /// <summary>
        /// Elimina una tarea por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la tarea a eliminar.</param>
        /// <returns>Objeto de respuesta con el resultado de la operación.</returns>
        public async Task<Response<Tareas>> DeleteAsync(int id)
        {
            var response = new Response<Tareas>();

            try
            {
                // Intenta obtener la tarea por su ID antes de eliminarla.
                var tareaToDelete = await _commonsProcess.GetIdAsync(id);
                if (tareaToDelete == null)
                {
                    response.Successful = false;
                    response.Message = "La tarea no existe.";
                    return response;
                }

                // Intenta eliminar la tarea usando el proceso común.
                var result = await _commonsProcess.DeleteAsync(id);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                // Si la operación fue exitosa:
                if (response.Successful)
                {
                    response.SingleData = tareaToDelete; // Asigna la tarea eliminada a la respuesta.
                    Delegates.NotificarEvento($"Tarea eliminada: {tareaToDelete.Description}"); // Notifica un evento.

                    _reactiveTask.OnTaskDeleted(tareaToDelete); // Emite un evento reactivo de eliminación de tarea.
                }
            }
            catch (Exception e)
            {
                // Captura cualquier excepción y agrega el mensaje de error a la respuesta.
                response.Errors.Add(e.Message);
            }

            return response; // Devuelve la respuesta.
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
                string clave = MemorizacionCache<object, List<Tareas>>.GenerarClave("pendientes");
                var cache = MemorizacionCache<object, List<Tareas>>.Obtener(clave);

                if (cache != null)
                {
                    response.DataList = cache;
                    response.Successful = true;
                    return response;
                }

                var all = await _commonsProcess.GetAllAsync();
                var filtradas = Delegates.FiltrarTareasPendientes(all);

                MemorizacionCache<object, List<Tareas>>.Guardar(clave, filtradas);

                response.DataList = filtradas;
                response.Successful = true;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }

            return response;
        }


        /// <summary>
        /// Get las tareas completadas.
        /// </summary>
        public async Task<Response<Tareas>> GetCompletadasAsync()
        {
            var response = new Response<Tareas>();

            try
            {
                string clave = MemorizacionCache<object, List<Tareas>>.GenerarClave("completadas");
                var cache = MemorizacionCache<object, List<Tareas>>.Obtener(clave);

                if (cache != null)
                {
                    response.DataList = cache;
                    response.Successful = true;
                    return response;
                }

                var all = await _commonsProcess.GetAllAsync();
                var filtradas = Delegates.FiltrarTareasCompletadas(all.ToList());

                MemorizacionCache<object, List<Tareas>>.Guardar(clave, filtradas);

                response.DataList = filtradas;
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
                string clave = MemorizacionCache<object, List<Tareas>>.GenerarClave("rango", desde.Date, hasta.Date);
                var cache = MemorizacionCache<object, List<Tareas>>.Obtener(clave);

                if (cache != null)
                {
                    response.DataList = cache;
                    response.Successful = true;
                    return response;
                }

                var all = await _commonsProcess.GetAllAsync();
                var filtradas = all
                    .Where(t => t.DueDate.Date >= desde.Date && t.DueDate.Date <= hasta.Date)
                    .ToList();

                MemorizacionCache<object, List<Tareas>>.Guardar(clave, filtradas);

                response.DataList = filtradas;
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
                string clave = MemorizacionCache<int, string>.GenerarClave(tareaId);
                var cache = MemorizacionCache<int, string>.Obtener(clave);

                if (cache != null)
                {
                    response.Message = cache;
                    response.Successful = true;
                    return response;
                }

                var tarea = await _commonsProcess.GetByIdAsync(tareaId);
                if (tarea == null)
                {
                    response.Successful = false;
                    response.Message = "Tarea no encontrada.";
                    return response;
                }

                int dias = Delegates.CalcularDiasRestantes(tarea.DueDate);
                string mensaje = $"Faltan {dias} dias para la tarea: {tarea.Description}";

                MemorizacionCache<int, string>.Guardar(clave, mensaje);

                response.Successful = true;
                response.Message = mensaje;
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }

            return response;
        }


    }
}