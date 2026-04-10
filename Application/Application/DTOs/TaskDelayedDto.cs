using System;

namespace GestorTarefa.Application.DTOs
{
    public class TaskDelayedDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string? Responsible { get; set; }
        public int DaysLate { get; set; }
    }
}
