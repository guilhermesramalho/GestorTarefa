using System.Linq;
using GestorTarefa.Application.DTOs;
using GestorTarefa.Application.Interfaces;
using GestorTarefa.Domain.Entities;
using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;
using Microsoft.EntityFrameworkCore;
using GestorTarefa.Domain.Enums;

namespace GestorTarefa.Application.Services
{
    public class TaskService : GestorTarefa.Application.Interfaces.ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TaskDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<TaskDto?> GetByIdAsync(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return null;
            return MapToDto(item);
        }

        public async Task<TaskDto> CreateAsync(TaskCreateDto dto)
        {
            var entity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                CreatedDate = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                Responsible = dto.Responsible
            };

            await _repository.CreateAsync(entity);
            return MapToDto(entity);
        }

        public async Task<bool> UpdateAsync(Guid id, TaskUpdateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            // Business rule: cannot mark as Completed without CompletionDate
            if (dto.Status == TaskStatus.Completed && dto.CompletionDate == null)
            {
                throw new InvalidOperationException("CompletionDate is required when marking a task as Completed.");
            }

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Status = dto.Status;
            existing.DueDate = dto.DueDate;
            existing.CompletionDate = dto.CompletionDate;
            existing.Priority = dto.Priority;
            existing.Responsible = dto.Responsible;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;
            await _repository.DeleteAsync(id);
            return true;
        }

        // Filtering & Reports
        public async Task<PagedResult<TaskDto>> FilterByStatusAsync(TaskStatus status, int page, int pageSize)
        {
            var query = _repository.Query().Where(x => x.Status == status);
            return await PaginateAndMapAsync(query, page, pageSize);
        }

        public async Task<PagedResult<TaskDto>> FilterByResponsavelAsync(string responsavel, int page, int pageSize)
        {
            var q = responsavel?.Trim().ToLower() ?? string.Empty;
            var query = _repository.Query().Where(x => x.Responsible != null && x.Responsible.ToLower().Contains(q));
            return await PaginateAndMapAsync(query, page, pageSize);
        }

        public async Task<PagedResult<TaskDto>> FilterByPeriodoAsync(DateTime start, DateTime end, int page, int pageSize)
        {
            var query = _repository.Query().Where(x => x.CreatedDate >= start && x.CreatedDate <= end);
            return await PaginateAndMapAsync(query, page, pageSize);
        }

        // New method for interface
        public async Task<PagedResult<TaskDto>> GetTasksByPeriodAsync(DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            // validations can be kept at service or controller level; just execute repository method
            var (data, total) = await _repository.GetByPeriodAsync(startDate, endDate, page, pageSize);
            var dtos = data.Select(x => MapToDto(x));
            return new PagedResult<TaskDto>
            {
                Data = dtos,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling((double)total / pageSize)
            };
        }

        public async Task<IEnumerable<TaskReportByStatusDto>> GetQuantityTasksByStatusAsync(TaskStatus status)
        {
            var groups = await _repository.Query()
                .Where(x => x.Status == status)
                .GroupBy(x => x.Status)
                .Select(g => new TaskReportByStatusDto { Status = g.Key.ToString(), Count = g.LongCount() })
                .ToListAsync();
            return groups;
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(TaskStatus status)
        {
            var groups = await _repository.Query()
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return groups;
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByPriorityAsync(TaskPriority priority)
        {
            var groups = await _repository.Query()
                .Where(x => x.Priority == priority)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return groups;
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByResponsibleAsync(string responsavel)
        {
            var groups = await _repository.Query()
                .Where(x => x.Responsible != null && x.Responsible.ToLower().Contains(responsavel.Trim().ToLower()))               
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
            return groups;
        }

        public async Task<PagedResult<TaskDelayedDto>> GetDelayedTasksAsync(int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var baseQuery = _repository.Query().Where(x => x.DueDate < now && x.Status != TaskStatus.Completed);

            var total = await baseQuery.LongCountAsync();
            var items = await baseQuery.OrderBy(x => x.DueDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var mapped = items.Select(x => new TaskDelayedDto
            {
                Id = x.Id,
                Title = x.Title,
                DueDate = x.DueDate,
                Responsible = x.Responsible,
                DaysLate = (int)(now - x.DueDate).TotalDays
            }).ToList();

            return new PagedResult<TaskDelayedDto>
            {
                Data = mapped,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling((double)total / pageSize)
            };
        }

        public async Task<TaskAverageCompletionDto> GetAverageCompletionTimeAsync()
        {
            var q = _repository.Query().Where(x => x.Status == TaskStatus.Completed && x.CompletionDate.HasValue);
            var list = await q.Select(x => new { x.CreatedDate, x.CompletionDate }).ToListAsync();
            if (!list.Any()) return new TaskAverageCompletionDto { AverageDays = 0, Count = 0 };
            var diffs = list.Select(p => (p.CompletionDate!.Value - p.CreatedDate).TotalDays);
            return new TaskAverageCompletionDto { AverageDays = diffs.Average(), Count = list.LongCount() };
        }

        private async Task<PagedResult<TaskDto>> PaginateAndMapAsync(IQueryable<TaskEntity> query, int page, int pageSize)
        {
            var total = await query.LongCountAsync();
            var items = await query.OrderBy(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var dtos = items.Select(x => MapToDto(x));

            return new PagedResult<TaskDto>
            {
                Data = dtos,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling((double)total / pageSize)
            };
        }

        private static TaskDto MapToDto(TaskEntity x)
        {
            return new TaskDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                CreatedDate = x.CreatedDate,
                DueDate = x.DueDate,
                CompletionDate = x.CompletionDate,
                Priority = x.Priority,
                Responsible = x.Responsible
            };
        }
    }
}
