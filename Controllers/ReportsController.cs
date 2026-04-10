using Microsoft.AspNetCore.Mvc;
using GestorTarefa.Application.Services;
using GestorTarefa.Application.DTOs;
using GestorTarefa.Domain.Enums;
using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;

namespace GestorTarefa.Controllers
{
    [ApiController]
    [Route("api/relatorios")]
    public class ReportsController : ControllerBase
    {
        private readonly TaskService _service;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(TaskService service, ILogger<ReportsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("qtd-tarefas-por-status/{status}")]
        public async Task<IActionResult> QuantityTasksByStatus(TaskStatus status)
        {
            _logger.LogInformation("GET /api/relatorios/tarefas-por-status");
            var result = await _service.GetQuantityTasksByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("tarefas-por-status/{status}")]
        public async Task<IActionResult> TasksByStatus(TaskStatus status)
        {
            _logger.LogInformation("GET /api/relatorios/tarefas-por-status");
            var result = await _service.GetTasksByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("tarefas-por-responsavel/{responsavel}")]
        public async Task<IActionResult> TasksByResponsible(string responsavel)
        {
            _logger.LogInformation("GET /api/relatorios/tarefas-por-responsavel");
            var result = await _service.GetTasksByResponsibleAsync(responsavel);
            return Ok(result);
        }

        [HttpGet("tarefas-atrasadas")]
        public async Task<IActionResult> DelayedTasks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET /api/relatorios/tarefas-atrasadas page={Page} pageSize={PageSize}", page, pageSize);
            var result = await _service.GetDelayedTasksAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("tempo-medio-conclusao")]
        public async Task<IActionResult> AverageCompletion()
        {
            _logger.LogInformation("GET /api/relatorios/tempo-medio-conclusao");
            var result = await _service.GetAverageCompletionTimeAsync();
            return Ok(result);
        }

        [HttpGet("tarefas-por-prioridade/{prioridade}")]
        public async Task<IActionResult> AverageCompletion(TaskPriority prioridade)
        {
            _logger.LogInformation("GET /api/relatorios/tarefas-por-prioridade");
            var result = await _service.GetTasksByPriorityAsync(prioridade);
            return Ok(result);
        }
    }
}
