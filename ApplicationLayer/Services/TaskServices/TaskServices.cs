using DomainLayer.Models;
using DomainLayer.Delegates;
using DomainLayer.Memory;
using InfrastructuraLayer.Repositorio.Commons;
using DomainLayer.DTO;
using ApplicationLayer.Services.Reactive;
using ApplicationLayer.Services.ColaServices;
using Microsoft.AspNetCore.SignalR;
using Proyect_TaskAPI.Hubs;

namespace ApplicationLayer.Services.TaskServices
{
    public class TaskServices
    {
        // Dependencias inyectadas cola de tareas y eventos reactivo
        private readonly ICommonsProccess<Tareas> _commonsProcess;
        private readonly ITaskQueueService _queueService;
        private readonly ReactiveTask _reactiveTask;
        private readonly IHubContext<TareaHub> _hubContext;

        // Constructor que inicializa las dependencias
        public TaskServices(
            ICommonsProccess<Tareas> commonsProcess,
            ITaskQueueService queueService,
            ReactiveTask reactiveTask,
            IHubContext<TareaHub> hubContext)
        {
            _commonsProcess = commonsProcess;
            _queueService = queueService;
            _reactiveTask = reactiveTask;
            _hubContext = hubContext;
        }

        //Obtiene una tarea por su ID.
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

        // Para obtener todas las tareas almacenadas.
        
        public async Task<Response<Tareas>> GetAllAsync()
        {
            var response = new Response<Tareas>();

            try
            {
                var result = await _commonsProcess.GetAllAsync(); 
                response.Successful = true;
                response.DataList = result.ToList(); 
                response.Message = "Lista de tareas obtenida correctamente.";
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Errors.Add(ex.Message);
            }

            return response;
        }



     
        // Add una nueva tarea de forma asincrona.
        
        public async Task<Response<Tareas>> AddAsync(TareasDTO dto)
        {
            var response = new Response<Tareas>();

            try
            {
                // Crea una nueva instancia de Tareas usando la clase DTO.
                var tarea = new Tareas
                {
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    Status = "pendiente", // Pondra el estado inicial como pendiente
                    AdditionalData = dto.AdditionalData ?? string.Empty 
                };

                // Valida la tarea antes de agregarla.
                if (!Delegates.ValidarTarea(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es valida. Verifica la descripcion y la fecha.";
                    return response;
                }

                // Intenta agregar la tarea
                var result = await _commonsProcess.AddAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (response.Successful)
                {
                    response.SingleData = tarea; 
                    Delegates.NotificarEvento($"Tarea agregada: {tarea.Description}");

                    // evento reactivo para cuando se cree una tarea.
                    _reactiveTask.OnTaskCreated(tarea); 

                    // Para notificar SignalR
                    await _hubContext.Clients.All.SendAsync("TareaCreada", tarea);

                    // Si el estado es pendiente, encola la tarea para procesamiento.
                    if (tarea.Status?.ToLowerInvariant() == "pendiente")
                        _queueService.Enqueue(tarea);
                }
            }
            catch (Exception ex)
            {
                // Captura cualquier excepcion y agrega el mensaje de error a la respuesta
                response.Errors.Add(ex.Message);
            }

            return response;
        }




        // Actualiza una tarea existente de forma asincrona
        public async Task<Response<Tareas>> UpdateAsync(Tareas tarea)
        {
            var response = new Response<Tareas>();

            try
            {
                // Intenta actualizar la tarea 
                var result = await _commonsProcess.UpdateAsync(tarea);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (response.Successful)
                {
                    // Emite un evento reactivo de actualizacion de tarea
                    _reactiveTask.OnTaskUpdated(tarea); 

                    // Para notificar SignalR
                    await _hubContext.Clients.All.SendAsync("TareaActualizada", tarea);

                    // Si el estado es pendient, encola la tarea.
                    if (tarea.Status?.ToLowerInvariant() == "pendiente")
                        _queueService.Enqueue(tarea);
                }
            }
            catch (Exception e)
            {
                // Captura cualquier excepcion y agrega el mensaje de error a la respuest
                response.Errors.Add(e.Message);
            }

            return response; 
        }








        // Elimina una tarea por su ID 
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

                // Intenta eliminar la tarea usando el proceso 
                var result = await _commonsProcess.DeleteAsync(id);
                response.Successful = result.IsSuccess;
                response.Message = result.Message;

                if (response.Successful)
                {
                    response.SingleData = tareaToDelete; // Asigna la tarea eliminada a la respuesta.
                    Delegates.NotificarEvento($"Tarea eliminada: {tareaToDelete.Description}"); // Notifica un evento.

                    _reactiveTask.OnTaskDeleted(tareaToDelete); // Emite un evento reactivo de que se elimino tarea.
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }

            return response;
        }
    

    // LINQ y Delegados
    // Obtiene las tareas pendientes.
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


        // Get las tareas completadas.
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



        /// Obtiene las tareas por rango de fecha de vencimiento
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


        /// Obtiene los dias restantes para una tarea.
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