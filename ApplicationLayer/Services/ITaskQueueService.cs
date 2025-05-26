using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public interface ITaskQueueService
    {
        void Enqueue(Tareas tarea);
        bool TryDequeue(out Tareas tarea);
        int GetQueueCount();

    }
}
