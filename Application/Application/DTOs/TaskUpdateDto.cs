using System;
using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;
using TaskPriority = GestorTarefa.Domain.Enums.TaskPriority;

namespace GestorTarefa.Application.DTOs
{
    public class TaskUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public TaskPriority Priority { get; set; }
        public string? Responsible { get; set; }
    }
}
