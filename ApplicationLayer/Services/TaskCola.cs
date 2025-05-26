using InfrastructuraLayer.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                // Para intentar sacar una tarea de la cola
                if (_queue.TryDequeue(out var tarea))
                {
                    // Crea un scope para obtener servicios con un ciclo de vida limitado 
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<TaskApiContext>();

                    // Busca la tarea completa en la base de datos
                    var tareaDb = await context.Tarea.FindAsync(tarea.Id);

                    // Procesa la tarea si existe y su estado es pendiente
                    if (tareaDb != null && tareaDb.Status?.ToLowerInvariant() == "pendiente")
                    {
                        // Actualiza el estado a en proceso.
                        tareaDb.Status = "en proceso";
                        await context.SaveChangesAsync(); // Guardo el cambio en DB.

                        // Simula el procesamiento de la tarea 3 segundos.
                        await Task.Delay(3000, stoppingToken);

                        // Actualiza el estado a completada.
                        tareaDb.Status = "completada";
                        await context.SaveChangesAsync(); // Guarda el cambio en DB.
                    }
                }
                else
                {
                    // Si la cola esta vacia, espera 1 segundo antes de revisar de nuevo.
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}