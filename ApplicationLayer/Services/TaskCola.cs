using InfrastructuraLayer.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class TaskCola : BackgroundService
    {
        private readonly ITaskQueueService _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public TaskCola(ITaskQueueService queue, IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var tarea))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<TaskApiContext>();

                    var tareaDb = await context.Tarea.FindAsync(tarea.Id);

                    if (tareaDb != null && tareaDb.Status?.ToLowerInvariant() == "pendiente")
                    {
                        // Espera 2 minutos antes de procesar
                        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

                        // Marca como en proceso
                        tareaDb.Status = "en proceso";
                        await context.SaveChangesAsync();

                        // Simula procesamiento durante 2 minuto
                        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

                        // Marca como completada
                        tareaDb.Status = "completada";
                        await context.SaveChangesAsync();
                    }
                }

                // Aqui para quee espere 1 segundo entre cada ciclo para evitar sobrecarga
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
