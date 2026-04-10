namespace GestorTarefa.Application.DTOs
{
    public class TaskReportByStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public long Count { get; set; }
    }
}
