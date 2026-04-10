using System;
using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;
using TaskPriority = GestorTarefa.Domain.Enums.TaskPriority;

namespace GestorTarefa.Application.DTOs
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public string? Responsible { get; set; }
    }
}
