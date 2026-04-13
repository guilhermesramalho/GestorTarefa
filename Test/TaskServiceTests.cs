using GestorTarefa.Application.Interfaces;
using GestorTarefa.Domain.Entities;
using Moq;

using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;
using TaskPriority = GestorTarefa.Domain.Enums.TaskPriority;

namespace Application.Tests.TaskService.Tests
{
    public class TaskServiceTests
    {
        [Fact]
        public async Task GetTasksByPeriodAsync_ValidDates_ReturnsPagedResult()
        {
            // Arrange
            var repoMock = new Mock<ITaskRepository>();
            var start = new DateTime(2025, 1, 1);
            var end = new DateTime(2025, 12, 31);
            var tasks = Enumerable.Range(1, 15).Select(i => new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = $"T{i}",
                CreatedDate = start.AddDays(i),
                DueDate = start.AddDays(i + 10),
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium
            }).ToList();

            repoMock.Setup(r => r.GetByPeriodAsync(start, end, 1, 10)).ReturnsAsync((tasks.Take(10), (long)tasks.Count));

            var service = new GestorTarefa.Application.Services.TaskService(repoMock.Object);

            // Act
            var result = await service.GetTasksByPeriodAsync(start, end, 1, 10);

            // Assert
            Assert.Equal(10, result.Data.Count());
            Assert.Equal(15, result.TotalRecords);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.TotalPages);
        }

        [Fact]
        public async Task GetTasksByPeriodAsync_NoData_ReturnsEmpty()
        {
            var repoMock = new Mock<ITaskRepository>();
            var start = new DateTime(2025, 1, 1);
            var end = new DateTime(2025, 12, 31);
            repoMock.Setup(r => r.GetByPeriodAsync(start, end, 1, 10)).ReturnsAsync((Enumerable.Empty<TaskEntity>(), 0L));
            var service = new GestorTarefa.Application.Services.TaskService(repoMock.Object);

            var result = await service.GetTasksByPeriodAsync(start, end, 1, 10);

            Assert.Empty(result.Data);
            Assert.Equal(0, result.TotalRecords);
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public async Task GetTasksByPeriodAsync_InvalidDates_Throws()
        {
            var repoMock = new Mock<ITaskRepository>();
            var service = new GestorTarefa.Application.Services.TaskService(repoMock.Object);
            var start = new DateTime(2026, 1, 2);
            var end = new DateTime(2025, 1, 1);

            await Assert.ThrowsAsync<ArgumentException>(() => service.GetTasksByPeriodAsync(start, end, 1, 10));
        }

        [Fact]
        public async Task GetTasksByPeriodAsync_MapsEntityToDto()
        {
            var repoMock = new Mock<ITaskRepository>();
            var start = new DateTime(2025, 1, 1);
            var end = new DateTime(2025, 12, 31);

            var entity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = "Titulo",
                Description = "Desc",
                CreatedDate = start.AddDays(1),
                DueDate = start.AddDays(5),
                Status = TaskStatus.InProgress,
                Priority = TaskPriority.High,
                Responsible = "Fulano"
            };

            repoMock.Setup(r => r.GetByPeriodAsync(start, end, 1, 10)).ReturnsAsync((new[] { entity }, 1L));
            var service = new GestorTarefa.Application.Services.TaskService(repoMock.Object);

            var result = await service.GetTasksByPeriodAsync(start, end, 1, 10);
            var dto = result.Data.First();

            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Title, dto.Title);
            Assert.Equal(entity.Description, dto.Description);
            Assert.Equal(entity.Status, dto.Status);
            Assert.Equal(entity.Priority, dto.Priority);
            Assert.Equal(entity.Responsible, dto.Responsible);
        }
    }
}
