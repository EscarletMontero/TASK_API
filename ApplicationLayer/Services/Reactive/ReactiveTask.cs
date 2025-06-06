using DomainLayer.Models;
using System;
using System.Reactive.Subjects;

namespace ApplicationLayer.Services.Reactive
{
    public class ReactiveTask : IDisposable
    {
        private readonly Subject<Tareas> _taskSubject = new();

        public IObservable<Tareas> TaskStream => _taskSubject;

        // Es pa notificar acciones en tareas:
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
            _taskSubject.OnNext(tarea);
        }

        public void Dispose()
        {
            _taskSubject?.Dispose();
        }
    }
}
