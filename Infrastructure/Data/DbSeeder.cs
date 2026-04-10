using GestorTarefa.Domain.Entities;
using TaskStatus = GestorTarefa.Domain.Enums.TaskStatus;
using TaskPriority = GestorTarefa.Domain.Enums.TaskPriority;
using Microsoft.EntityFrameworkCore;

namespace GestorTarefa.Infrastructure.Data
{
    public class DbSeeder
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DbSeeder> _logger;
        private static readonly string[] Names = new[]
        {
            "Ana", "Bruno", "Carlos", "Daniela", "Eduardo", "Fernanda", "Gabriel", "Helena", "Igor", "Juliana",
            "Kleber", "Larissa", "Marcos", "Natalia", "Otavio", "Patricia", "Ricardo", "Sofia", "Tiago", "Vanessa"
        };

        public DbSeeder(AppDbContext context, ILogger<DbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            if (await _context.Tasks.AnyAsync())
            {
                _logger.LogInformation("Database already contains tasks. Skipping seeding.");
                return;
            }

            var rnd = new Random();
            var list = new List<TaskEntity>();
            for (int i = 0; i < 100; i++)
            {
                var status = (TaskStatus)rnd.Next(0, 3);
                DateTime created = DateTime.UtcNow.AddDays(-rnd.Next(1, 365));
                DateTime due = DateTime.UtcNow.AddDays(rnd.Next(-30, 90));
                DateTime? completion = null;
                if (status == TaskStatus.Completed)
                {
                    completion = created.AddDays(rnd.Next(1, 30));
                    if (completion > DateTime.UtcNow) completion = DateTime.UtcNow;
                }

                var priority = (TaskPriority)rnd.Next(0, 3);
                var responsible = Names[rnd.Next(Names.Length)];

                list.Add(new TaskEntity
                {
                    Id = Guid.NewGuid(),
                    Title = $"Tarefa {i + 1} - {priority}",
                    Description = $"Descricao da tarefa {i + 1}",
                    Status = status,
                    CreatedDate = created,
                    DueDate = due,
                    CompletionDate = completion,
                    Priority = priority,
                    Responsible = responsible
                });
            }

            _context.Tasks.AddRange(list);
            var inserted = await _context.SaveChangesAsync();
            _logger.LogInformation("Seed completed. Inserted {Inserted} entities.", inserted);
        }
    }
}
