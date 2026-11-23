
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

        // Entrenador responsable de la rutina
        // 0 significa que todavía no tiene entrenador
        public int TrainerId { get; set; }

        public Routine()
        {
        }

        // Constructor existente
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

        // Constructor con entrenador asignado
        public Routine(
            int id,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate,
            int trainerId)
        {
            Id = id;
            Name = name;
            Description = description;
            Duration = duration;
            StartDate = startDate;
            EndDate = endDate;
            TrainerId = trainerId;
        }
    }
}
