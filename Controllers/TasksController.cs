using Microsoft.AspNetCore.Mvc;
using GestorTarefa.Application.Services;
using GestorTarefa.Application.DTOs;
using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;

namespace GestorTarefa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly GestorTarefa.Application.Interfaces.ITaskService _service;
        private readonly ILogger<TasksController> _logger;

        public TasksController(GestorTarefa.Application.Interfaces.ITaskService service, ILogger<TasksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync(); 
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TaskUpdateDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("status")]
        public async Task<IActionResult> ByStatus([FromQuery] TaskStatus status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET /api/task/status - status={Status} page={Page} pageSize={PageSize}", status, page, pageSize);
            var result = await _service.FilterByStatusAsync(status, page, pageSize);
            return Ok(result);
        }

        [HttpGet("responsavel")]
        public async Task<IActionResult> ByResponsavel([FromQuery] string responsavel, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET /api/task/responsavel - responsavel={Responsavel} page={Page} pageSize={PageSize}", responsavel, page, pageSize);
            var result = await _service.FilterByResponsavelAsync(responsavel, page, pageSize);
            return Ok(result);
        }

        [HttpGet("periodo")]
        public async Task<IActionResult> ByPeriodo([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            _logger.LogInformation("GET /api/task/periodo - start={Start} end={End} page={Page} pageSize={PageSize}", startDate, endDate, page, pageSize);

            if (!startDate.HasValue || !endDate.HasValue)
                return BadRequest(new { message = "startDate and endDate are required." });

            if (startDate > endDate)
                return BadRequest(new { message = "startDate must be less than or equal to endDate." });

            if (!page.HasValue || !pageSize.HasValue || page < 1 || pageSize < 1)
                return BadRequest(new { message = "page and pageSize are required and must be greater than zero." });

            var result = await _service.GetTasksByPeriodAsync(startDate.Value, endDate.Value, page.Value, pageSize.Value);
            return Ok(result);
        }
    }
}
