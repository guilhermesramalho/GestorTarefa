using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GestorTarefa.Infrastructure.Data;

#nullable disable

namespace GestorTarefa.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("GestorTarefa.Domain.Entities.TaskEntity", b =>
            {
                b.Property<Guid>("Id")
                    .HasColumnType("uuid");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.Property<string>("Description")
                    .HasMaxLength(1000)
                    .HasColumnType("character varying(1000)");

                b.Property<int>("Status")
                    .HasColumnType("integer");

                b.Property<DateTime>("CreatedDate")
                    .HasColumnType("timestamp without time zone");

                b.Property<DateTime>("DueDate")
                    .HasColumnType("timestamp without time zone");

                b.Property<DateTime?>("CompletionDate")
                    .HasColumnType("timestamp without time zone");

                b.Property<int>("Priority")
                    .HasColumnType("integer");

                b.Property<string>("Responsible")
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.HasKey("Id");

                b.ToTable("tasks");
            });
        }
    }
}
