
using System;

namespace TaymotiveModels.Models
{
    public class Routine
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // Entrenador responsable de la rutina.
        // 0 significa que no tiene entrenador asignado.
        public int TrainerId { get; set; }

        // Cantidad de series y repeticiones.
        // El valor 0 permite conservar rutinas antiguas
        // que todavía no tienen estos datos registrados.
        public int Sets { get; set; }

        public int Repetitions { get; set; }

        public Routine()
        {
        }

        // Constructor original: se conserva por compatibilidad.
        public Routine(
            int id,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Duration = duration;
            StartDate = startDate;
            EndDate = endDate;
            TrainerId = 0;
        }

        // Constructor original con entrenador asignado.
        public Routine(
            int id,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate,
            int trainerId)
            : this(id, name, description, duration, startDate, endDate)
        {
            TrainerId = trainerId;
        }

        // Constructor con series y repeticiones.
        public Routine(
            int id,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate,
            int trainerId,
            int sets,
            int repetitions)
            : this(id, name, description, duration, startDate, endDate, trainerId)
        {
            Sets = sets;
            Repetitions = repetitions;
        }
    }
}
