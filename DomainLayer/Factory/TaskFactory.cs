using DomainLayer.Models;

namespace DomainLayer.Factory
{
    //Puse comentarios de documentacion XML para no perderme mas adelante

    public static class TaskFactory
        {
            public static Tareas Create(
                string description,
                DateTime dueDate,
                string status = "Pendiente",
                string additionalData = "")
            {
                if (string.IsNullOrWhiteSpace(description))
                    throw new ArgumentException("La descripcion no puede estar vacia.");

                if (dueDate.Date < DateTime.Now.Date)
                    throw new ArgumentException("La fecha de vencimiento no puede ser en el pasado.");

                return new Tareas
                {
                    Description = description.Trim(),
                    DueDate = dueDate,
                    Status = status.Trim(),
                    AdditionalData = additionalData?.Trim()
                };
            }

            /// <summary>
            /// Crea una tarea rapida para pruebas o ejemplo.
            /// </summary>
            public static Tareas CreateDemoTask()
            {
                return new Tareas
                {
                    Description = "Demo Task",
                    DueDate = DateTime.Now.AddDays(7),
                    Status = "Pendiente",
                    AdditionalData = "Tarea generada automaticamente para muestra."
                };
            }

            /// <summary>
            /// Crea una tarea de alta prioridad.
            /// </summary>
            /// <param name="description">Descripcion de la tarea.</param>
            /// <returns>Objeto Tareas de alta prioridad.</returns>
            public static Tareas CreateHighPriorityTask(string description)
            {
                return new Tareas
                {
                    Description = description.Trim(),
                    DueDate = DateTime.Now.AddDays(1),
                    Status = "Pendiente",
                    AdditionalData = "High Priority"
                };
            }
        }
    }
