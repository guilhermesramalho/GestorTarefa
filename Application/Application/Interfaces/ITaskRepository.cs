using GestorTarefa.Domain.Entities;

namespace GestorTarefa.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(Guid id);
        Task CreateAsync(TaskEntity task);
        Task UpdateAsync(TaskEntity task);
        Task DeleteAsync(Guid id);
        IQueryable<TaskEntity> Query();
        Task<(IEnumerable<TaskEntity> Data, long TotalRecords)> GetByPeriodAsync(DateTime startDate, DateTime endDate, int page, int pageSize);
    }
}
