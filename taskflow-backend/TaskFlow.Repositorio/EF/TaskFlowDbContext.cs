using Microsoft.EntityFrameworkCore;
using TaskFlow.Dominio.Classe;
using TaskFlow.Repositorio.EF.Maps;

namespace TaskFlow.Repositorio.EF;

public sealed class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TarefaConfig());
    }
}
