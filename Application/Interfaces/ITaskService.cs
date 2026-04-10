using GestorTarefa.Application.DTOs;
using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;

namespace GestorTarefa.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetAllAsync();
        Task<TaskDto?> GetByIdAsync(Guid id);
        Task<TaskDto> CreateAsync(TaskCreateDto dto);
        Task<bool> UpdateAsync(Guid id, TaskUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);

        Task<PagedResult<TaskDto>> FilterByStatusAsync(TaskStatus status, int page, int pageSize);
        Task<PagedResult<TaskDto>> FilterByResponsavelAsync(string responsavel, int page, int pageSize);
        Task<PagedResult<TaskDto>> GetTasksByPeriodAsync(DateTime startDate, DateTime endDate, int page, int pageSize);
    }
}
