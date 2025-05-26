using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using InfrastructuraLayer.Context;
using ApplicationLayer.Services.TaskServices;
using DomainLayer.Models;
using InfrastructuraLayer.Repositorio.Commons;
using InfrastructuraLayer.Repositorio.TaskRepository;
using ApplicationLayer.Services; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuro DbContext
builder.Services.AddDbContext<TaskApiContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskDB"));
});

// Registra los servicios de repositorio y logica de negocio CRUD
builder.Services.AddScoped<ICommonsProccess<Tareas>, TaskRepository>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddSingleton<ITaskQueueService, TaskQueueService>();

// Registre mi TaskCola como un Hosted Service Background Service
builder.Services.AddHostedService<TaskCola>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica migraciones pendientes a la base de datos al iniciar la aplicación.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TaskApiContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();