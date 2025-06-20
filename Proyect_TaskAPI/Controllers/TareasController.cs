// Proyect_TaskAPI/Controllers/TareasController.cs
using ApplicationLayer.Services.TaskServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;
using DomainLayer.Memory;
using ApplicationLayer.Services.ColaServices;

namespace Proyect_TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly TaskServices _service;
        private readonly ITaskQueueService _tareaQueueService;

        public TareasController(TaskServices service, ITaskQueueService tareaQueueService)
        {
            _service = service;
            _tareaQueueService = tareaQueueService;
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
        [HttpPost]
        public async Task<ActionResult<Response<Tareas>>> AddAsync([FromBody] TareasDTO tareaDTO)
        {
            var response = await _service.AddAsync(tareaDTO);
            if (response.Successful) return Ok(response);
            return BadRequest(response);
        }

        // Actualizar: api/tareas
        // Actualiza una tarea, procede a ser reactividad y encola si es pendiente
        [HttpPut]
        public async Task<ActionResult<Response<Tareas>>> UpdateAsync(Tareas tarea)
        {
            var response = await _service.UpdateAsync(tarea);
            if (response.Successful) return Ok(response);
            return BadRequest(response);
        }

        // DELETE: api/tareas/id
        // Elimina una tarea y emite evento reactivo de eliminacion
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<Tareas>>> DeleteAsync(int id)
        {
            var response = await _service.DeleteAsync(id);
            if (response.Successful) return Ok(response);
            return BadRequest(response);
        }

 
        // GET: api/tareas/queue/count
        // Me dara la cantidad de tareas actualmente en la cola
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
        // Dara solo las tareas con estado pendiente
        [HttpGet("pendientes")]
        public async Task<ActionResult<Response<Tareas>>> GetPendientesAsync()
            => await _service.GetPendientesAsync();

        // GET: api/tareas/completadas
        // Devuelve solo las tareas con estado completadas
        [HttpGet("completadas")]
        public async Task<ActionResult<Response<Tareas>>> GetCompletadasAsync()
            => await _service.GetCompletadasAsync();

        // GET: api/tareas/rango?desde=2024-01-01 hasta=2024-12-31
        // Devuelve las tareas en un rango de fechas
        [HttpGet("rango")]
        public async Task<ActionResult<Response<Tareas>>> GetPorRangoFechaAsync([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => await _service.GetPorRangoFechaAsync(desde, hasta);

        // GET: api/tareas/id/diasrestantes
        [HttpGet("{id}/diasrestantes")]
        public async Task<ActionResult<Response<string>>> GetDiasRestantesAsync(int id)
            => await _service.GetDiasRestantesAsync(id);



        //Esto es por si acaso, un borradorsito 
        [HttpDelete("cache/limpiar")]
        public IActionResult LimpiarCache()
        {
            MemorizacionCache<string, object>.Limpiar(); 
            return Ok("Cache limpiada exitosamente.");
        }

    }
}
