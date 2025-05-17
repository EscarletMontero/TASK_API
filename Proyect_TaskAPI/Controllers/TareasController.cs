// Proyect_TaskAPI/Controllers/TareasController.cs
using ApplicationLayer.Services.TaskServices;
using DomainLayer.DTO;
using DomainLayer.Models;
using DomainLayer.Factory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFactory = DomainLayer.Factory.TaskFactory;

namespace Proyect_TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly TaskService _service;
        public TareasController(TaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<Response<Tareas>>> GetAllAsync()
            => await _service.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Tareas>>> GetByIdAsync(int id)
            => await _service.GetByIdAsync(id);

        // Aquí se recibe el TareasDTO
        [HttpPost]
        public async Task<ActionResult<Response<Tareas>>> AddAsync([FromBody] TareasDTO tareaDTO)
        {
            var response = await _service.AddAsync(
                tareaDTO.Description,
                tareaDTO.DueDate,
                tareaDTO.Status ?? "Pendiente",
                tareaDTO.AdditionalData ?? "" 
            );

            if (response.Successful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPut]
        public async Task<ActionResult<Response<Tareas>>> UpdateAsync(Tareas tarea)
            => await _service.UpdateAsync(tarea);

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<Tareas>>> DeleteAsync(int id)
            => await _service.DeleteAsync(id);

        // Endpoints extra con linq

        [HttpGet("pendientes")]
        public async Task<ActionResult<Response<Tareas>>> GetPendientesAsync()
            => await _service.GetPendientesAsync();

        [HttpGet("vencidas")]
        public async Task<ActionResult<Response<Tareas>>> GetVencidasAsync()
            => await _service.GetVencidasAsync();

        [HttpGet("rango")]
        public async Task<ActionResult<Response<Tareas>>> GetPorRangoFechaAsync([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
            => await _service.GetPorRangoFechaAsync(desde, hasta);

        [HttpGet("{id}/diasrestantes")]
        public async Task<ActionResult<Response<string>>> GetDiasRestantesAsync(int id)
            => await _service.GetDiasRestantesAsync(id);

        [HttpGet("con-adicionales")]
        public async Task<ActionResult<Response<Tareas>>> GetConDatosAdicionalesAsync()
            => await _service.GetConDatosAdicionalesAsync();
    }
}