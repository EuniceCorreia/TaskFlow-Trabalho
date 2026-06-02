using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Aplicacao;
using TaskFlow.API.Middlewares;
using TaskFlow.Aplicacao.Aplic;
using TaskFlow.Aplicacao.IAplic;
using TaskFlow.Dominio.IRep;
using TaskFlow.Dominio.IServ;
using TaskFlow.Dominio.Serv;
using TaskFlow.Repositorio.EF;
using TaskFlow.Repositorio.Rep;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<ITarefaRep, TarefaRep>();
builder.Services.AddScoped<ITarefaServ, TarefaServ>();
builder.Services.AddScoped<IMapperTarefa, MapperTarefa>();
builder.Services.AddScoped<ITarefaAplic, TarefaAplic>();

var connectionString = builder.Configuration.GetConnectionString("TaskFlow");
builder.Services.AddDbContext<TaskFlowDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<TratamentoExcecoesMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DefaultModelsExpandDepth(-1));
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();
