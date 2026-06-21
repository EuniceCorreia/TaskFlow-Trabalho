using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Dominio.Classe;

namespace TaskFlow.Repositorio.EF.Maps;

public sealed class TarefaConfig : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.ToTable("Tarefas");

        builder.HasKey( t => t.Id);

        builder.Property( t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property( t => t.Titulo)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property( t => t.Descricao)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property( t => t.Disciplina)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property( t => t.Prioridade)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property( t => t.DataEntrega)
            .IsRequired();

        builder.Property( t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property( t => t.CriadaEm)
            .IsRequired();

        builder.Property( t => t.AtualizadaEm);

        builder.HasIndex( t => t.Status);
        builder.HasIndex( t => t.DataEntrega);
        builder.HasIndex( t => new { t.Disciplina, t.Titulo })
            .IsUnique();
    }
}
