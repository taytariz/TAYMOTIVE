
using System;

namespace TaymotiveModels.Models
{
    public class RoutineProgress
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int RoutineId { get; set; }

        public string ExerciseName { get; set; } = string.Empty;

        public double Weight { get; set; }

        public DateTime ProgressDate { get; set; }

        public RoutineProgress()
        {
        }

        public RoutineProgress(
            int id,
            int userId,
            int routineId,
            string exerciseName,
            double weight,
            DateTime progressDate)
        {
            Id = id;
            UserId = userId;
            RoutineId = routineId;
            ExerciseName = exerciseName;
            Weight = weight;
            ProgressDate = progressDate;
        }
    }
}
