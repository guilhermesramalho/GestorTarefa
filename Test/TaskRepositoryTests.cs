using GestorTarefa.Domain.Entities;
using GestorTarefa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;
using TaskPriority = GestorTarefa.Domain.Enums.TaskPriority;

namespace Infrastructure.Tests.TaskRepository.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;

        public TaskRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;


            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task GetByPeriodAsync_FiltersAndPaginates()
        {
            // Arrange
            var start = new DateTime(2025, 1, 1);
            for (int i = 0; i < 25; i++)
            {
                _context.Tasks.Add(new TaskEntity
                {
                    Id = Guid.NewGuid(),
                    Title = $"T{i}",
                    CreatedDate = start.AddDays(i),
                    DueDate = start.AddDays(i + 1),
                    Status = TaskStatus.Pending,
                    Priority = TaskPriority.Low
                });
            }
            await _context.SaveChangesAsync();

            var repo = new GestorTarefa.Infrastructure.Repositories.TaskRepository(_context);

            // Act
            var (data, total) = await repo.GetByPeriodAsync(start.AddDays(5), start.AddDays(19), 2, 5);

            // Assert
            Assert.Equal(15, total); // from day 5 to 19 inclusive -> 15 days
            Assert.Equal(5, data.Count()); // page 2 size 5 -> items 6-10
            Assert.Equal(start.AddDays(10).Date, data.First().CreatedDate.Date);
        }

        [Fact]
        public async Task GetByPeriodAsync_ReturnsTotalCorrectly()
        {
            var start = new DateTime(2025, 1, 1);
            for (int i = 0; i < 12; i++)
            {
                _context.Tasks.Add(new TaskEntity
                {
                    Id = Guid.NewGuid(),
                    Title = $"T{i}",
                    CreatedDate = start.AddDays(i),
                    DueDate = start.AddDays(i + 1),
                    Status = TaskStatus.Pending,
                    Priority = TaskPriority.Low
                });
            }
            await _context.SaveChangesAsync();

            var repo = new GestorTarefa.Infrastructure.Repositories.TaskRepository(_context);

            var (data, total) = await repo.GetByPeriodAsync(start, start.AddDays(11), 1, 10);
            Assert.Equal(12, total);
            Assert.Equal(10, data.Count());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
