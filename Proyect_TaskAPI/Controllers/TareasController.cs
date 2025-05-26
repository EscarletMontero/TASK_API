// Proyect_TaskAPI/Controllers/TareasController.cs
using ApplicationLayer.Services.TaskServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using ApplicationLayer.Services;
using ApplicationLayer.Services.Reactive;

namespace Proyect_TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly TaskService _service;
        private readonly ReactiveTask _reactive; // Servicio que nootifica REACTIVO
        private readonly ITaskQueueService _tareaQueueService; // Servicio de cola que procesa tareas pendientes

        public TareasController(TaskService service, ITaskQueueService tareaQueueService, ReactiveTask reactiveQueue)
        {
            _service = service;
            _tareaQueueService = tareaQueueService;
            _reactive = reactiveQueue;
        }

        // GET: api/tareas
        [HttpGet]
        public async Task<ActionResult<Response<Tareas>>> GetAllAsync()
            => await _service.GetAllAsync();

        // GET: api/tareas/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Tareas>>> GetByIdAsync(int id)
            => await _service.GetByIdAsync(id);

        // POST: api/tareas
        // CREA una nueva tarea, emite evento reactivo, y si es "pendiente" la encola.
        [HttpPost]
        public async Task<ActionResult<Response<Tareas>>> AddAndEnqueueTaskAsync([FromBody] TareasDTO tareaDTO)
        {
            var response = await _service.AddAsync(tareaDTO);

            if (response.Successful && response.SingleData != null)
            {
                // Emite evento de creacion reactiva
                _reactive.OnTaskCreated(response.SingleData);

                // Encola si el estado sigue siendo "pendiente"
                if (response.SingleData.Status?.ToLowerInvariant() == "pendiente")
                {
                    _tareaQueueService.Enqueue(response.SingleData);
                }

                return Ok(new Response<Tareas>(true, "Tarea creada y agregada a la cola para procesamiento.", response.SingleData));
            }

            return BadRequest(response);
        }


        // PUT: api/tareas
        // Actualiza una tarea, emite reactividad y encola si es "pendiente"
        [HttpPut]
        public async Task<ActionResult<Response<Tareas>>> UpdateAsync(Tareas tarea)
        {
            var response = await _service.UpdateAsync(tarea);

            if (response.Successful && response.SingleData != null)
            {
                // Emite evento de actualizacion reactiva
                _reactive.OnTaskUpdated(response.SingleData);

                // Encola solo si el estado es pendiente
                if (response.SingleData.Status?.ToLowerInvariant() == "pendiente")
                {
                    _tareaQueueService.Enqueue(response.SingleData); // Encola
                }
            }

            return response.Successful ? Ok(response) : BadRequest(response);
        }

        // DELETE: api/tareas/id
        // Elimina una tarea y emite evento reactivo de eliminacion
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<Tareas>>> DeleteAsync(int id)
        {
            var response = await _service.DeleteAsync(id);

            if (response.Successful && response.SingleData != null)
            {
                // Emitir evento de eliminacion reactiva
                _reactive.OnTaskDeleted(response.SingleData); //Notifica la eliminacion
            }

            return response.Successful ? Ok(response) : BadRequest(response);
        }

        // GET: api/tareas/queue/count
        // Devuelve la cantidad de tareas actualmente en la cola
        [HttpGet("queue/count")]
        public IActionResult GetQueueCount()
        {
            if (_tareaQueueService is TaskQueueService concreteQueueService)
            {
                int cantidad = concreteQueueService.GetQueueCount();
                return Ok(new { tareasEnCola = cantidad });
            }

            return StatusCode(500, new { error = "Servicio de cola no disponible." });
        }


        // GET: api/tareas/pendientes
        // Devuelve solo las tareas con estado "pendiente"
        [HttpGet("pendientes")]
        public async Task<ActionResult<Response<Tareas>>> GetPendientesAsync()
            => await _service.GetPendientesAsync();

        // GET: api/tareas/vencidas
        // Devuelve las tareas que ya vencieron
        [HttpGet("vencidas")]
        public async Task<ActionResult<Response<Tareas>>> GetVencidasAsync()
            => await _service.GetVencidasAsync();

        // GET: api/tareas/rango?desde=2024-01-01 hasta=2024-12-31
        // Devuelve las tareas en un rango de fechas
        [HttpGet("rango")]
        public async Task<ActionResult<Response<Tareas>>> GetPorRangoFechaAsync([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => await _service.GetPorRangoFechaAsync(desde, hasta);

        // GET: api/tareas/id/diasrestantes
        // Devuelve cuantos días faltan para que venza la tarea
        [HttpGet("{id}/diasrestantes")]
        public async Task<ActionResult<Response<string>>> GetDiasRestantesAsync(int id)
            => await _service.GetDiasRestantesAsync(id);

    }
}
