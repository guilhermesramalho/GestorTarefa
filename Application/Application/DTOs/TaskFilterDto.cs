using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;

namespace GestorTarefa.Application.DTOs
{
    public class TaskFilterDto
    {
        public TaskStatus? Status { get; set; }
        public string? Responsavel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
