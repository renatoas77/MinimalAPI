using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Context;
using MinimalAPI.Models;
using System.Reflection.Metadata.Ecma335;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Tarefas"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("frases", async () => await new HttpClient().GetStringAsync("https://ron-swanson-quotes.herokuapp.com/v2/quotes"));

app.MapPost("Tarefa", async (AppDbContext context, Tarefa tarefa) =>
{
    context.Tarefas.Add(tarefa);
    await context.SaveChangesAsync();
    return Results.Created("/Tarefa", tarefa);
});

app.MapGet("Tarefa", async (AppDbContext context, int id) => await context.Tarefas.FirstOrDefaultAsync(T => T.Id == id));

app.MapGet("Tarefas", async (AppDbContext context) => await context.Tarefas.ToListAsync());

app.MapGet("Tarefas/Concluidas", async (AppDbContext context) => await context.Tarefas.Where(t => t.IsConcluida).ToListAsync());

app.MapPut("Tarefa", async (AppDbContext context, Tarefa tarefa, int id) =>
{
    if(tarefa.Id != id)
    {
        return Results.BadRequest();
    }
    else
    {
        context.Entry(tarefa).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return Results.Ok(tarefa);
    }

});

app.MapDelete("Tarefa", async (AppDbContext context, int id) =>
{
    var tarefa = context.Tarefas.FirstOrDefault(ta => ta.Id == id);
    if(tarefa == null)
    {
        return Results.NotFound();
    }
    else
    {
        context.Tarefas.Remove(tarefa);
        await context.SaveChangesAsync();
        return Results.Ok(tarefa);
    }
});

app.Run();