using Microsoft.EntityFrameworkCore;
using InfrastructuraLayer.Context;
using ApplicationLayer.Services.TaskServices;
using DomainLayer.Models;
using InfrastructuraLayer.Repositorio.Commons;
using InfrastructuraLayer.Repositorio.TaskRepository;
using ApplicationLayer.Services.Reactive;
using ApplicationLayer.Services.ColaServices;
using ApplicationLayer.Services.JwtService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Proyect_TaskAPI.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5186); // HTTP
    options.ListenLocalhost(7165, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

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
//Reactive
builder.Services.AddSingleton<ReactiveTask>();
builder.Services.AddSingleton<ITaskQueueService, TaskQueueService>();
//Servicio para JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<JwtService>();

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

app.MapHub<TareaHub>("/tareasHub");

app.Run();
