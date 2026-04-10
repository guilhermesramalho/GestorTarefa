using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GestorTarefa.Domain.Entities;
using GestorTarefa.Domain.Enums;

namespace GestorTarefa.Infrastructure.Mappings
{
    public class TaskMap : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.ToTable("tasks");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.CompletionDate)
                .IsRequired(false);

            builder.Property(x => x.Priority)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Responsible)
                .HasMaxLength(200);
        }
    }
}
