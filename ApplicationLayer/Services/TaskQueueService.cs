// ApplicationLayer/Services/TaskQueueService.cs
using ApplicationLayer.Services.Reactive;
using DomainLayer.Models;
using System.Collections.Concurrent;
using System.Reactive.Linq;

namespace ApplicationLayer.Services
{
    public class TaskQueueService : ITaskQueueService, IDisposable
    {
        private readonly ConcurrentQueue<Tareas> _cola = new();
        private readonly IDisposable _subscription;

        public TaskQueueService(ReactiveTask reactiveTask)
        {
            // Se suscribe al flujo reactivo y encola tareas que estén pendientes
            _subscription = reactiveTask.TaskStream
                .Where(t => t.Status?.ToLowerInvariant() == "pendiente")
                .Subscribe(tarea => Enqueue(tarea));
        }

        public void Enqueue(Tareas tarea)
        {
            _cola.Enqueue(tarea);
        }

        public bool TryDequeue(out Tareas tarea)
        {
            return _cola.TryDequeue(out tarea);
        }

        public int GetQueueCount()
        {
            return _cola.Count;
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
