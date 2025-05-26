using DomainLayer.Models;
using System;
using System.Reactive.Subjects;

namespace ApplicationLayer.Services.Reactive
{
    public class ReactiveTask : IDisposable
    {
        private readonly Subject<Tareas> _taskSubject = new();

        public IObservable<Tareas> TaskStream => _taskSubject;

        // Métodos para notificar acciones en tareas:
        public void OnTaskCreated(Tareas tarea)
        {
            _taskSubject.OnNext(tarea);
        }

        public void OnTaskUpdated(Tareas tarea)
        {
            _taskSubject.OnNext(tarea);
        }

        public void OnTaskDeleted(Tareas tarea)
        {
            // Opcional, según si quieres emitir también al eliminar
            _taskSubject.OnNext(tarea);
        }

        public void Dispose()
        {
            _taskSubject?.Dispose();
        }
    }
}
