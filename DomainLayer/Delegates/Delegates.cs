using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Models;

namespace DomainLayer.Delegates
{
    public static class Delegates
    {
        // 1. Delegado explicito para validar una tarea
        public delegate bool ValidateTaskDelegate(Tareas tarea);
        public static ValidateTaskDelegate ValidarTarea = tarea =>
            !string.IsNullOrWhiteSpace(tarea.Description) &&
            tarea.DueDate > DateTime.Now;

        // 2. Delegado explicito para notificaciones de eventos
        public delegate void NotificarEventoDelegate(string mensaje);
        public static NotificarEventoDelegate NotificarEvento = mensaje =>
            Console.WriteLine($"[NOTIFICACION] {mensaje}");

        // FUNC / ACTION        

        // 3. Func para calcular los dias restantes para una tarea
        public static Func<DateTime, int> CalcularDiasRestantes = fechaVencimiento =>
            (fechaVencimiento.Date - DateTime.Now.Date).Days;

        // 4. Func para filtrar tareas por vencer en X dias
        public static Func<List<Tareas>, int, List<Tareas>> FiltrarTareasPorVencer = (tareas, dias) =>
            tareas.Where(t =>
                (t.DueDate.Date - DateTime.Now.Date).Days <= dias &&
                t.Status?.ToLower() == "pending").ToList();

        // 5. Func para buscar por palabra clave en la descripcion
        public static Func<List<Tareas>, string, List<Tareas>> BuscarPorDescripcion = (tareas, palabraClave) =>
            tareas.Where(t => t.Description?.ToLower().Contains(palabraClave.ToLower()) == true).ToList();


        // LINQ 
        // 6. Metodo para filtrar tareas pendientes
        public static List<Tareas> FiltrarTareasPendientes(IEnumerable<Tareas> tareas)
        {
            return tareas.Where(t => t.Status?.ToLowerInvariant() == "pendiente").ToList();
        }

        // 7. Metodo para filtrar tareas vencidas
        public static List<Tareas> FiltrarTareasVencidas(IEnumerable<Tareas> tareas)
        {
            return tareas.Where(t =>
                t.DueDate.Date < DateTime.Now.Date ||
                t.Status?.ToLower() == "vencida").ToList();
        }

        // 8. Metodo para filtrar tareas completadas
        public static List<Tareas> FiltrarTareasCompletadas(List<Tareas> tareas)
        {
            return tareas.Where(t => t.Status?.ToLower() == "completed").ToList();
        }
    }
}