using GestorTarefa.Application.Interfaces;
using GestorTarefa.Domain.Entities;
using GestorTarefa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestorTarefa.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task CreateAsync(TaskEntity task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskEntity task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Tasks.FindAsync(id);
            if (entity == null) return;
            _context.Tasks.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<TaskEntity> Query()
        {
            return _context.Tasks.AsQueryable();
        }

        public async Task<(IEnumerable<TaskEntity> Data, long TotalRecords)> GetByPeriodAsync(DateTime startDate, DateTime endDate, int page, int pageSize)
        {
            var query = _context.Tasks.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);
            var total = await query.LongCountAsync();
            var data = await query.OrderBy(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (data, total);
        }
    }
}
