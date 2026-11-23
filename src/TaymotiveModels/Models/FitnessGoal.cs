
using System;

namespace TaymotiveModels.Models
{
    public class FitnessGoal
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string GoalType { get; set; } = string.Empty;

        public double InitialValue { get; set; }

        public double TargetValue { get; set; }

        public double CurrentValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; }

        public FitnessGoal()
        {
        }

        public FitnessGoal(
            int id,
            int userId,
            string goalType,
            double initialValue,
            double targetValue,
            DateTime startDate,
            DateTime endDate)
        {
            Id = id;
            UserId = userId;
            GoalType = goalType;
            InitialValue = initialValue;
            TargetValue = targetValue;
            CurrentValue = initialValue;
            StartDate = startDate;
            EndDate = endDate;
            IsCompleted = false;
        }

        public double CalculateProgress()
        {
            double totalChange =
                TargetValue - InitialValue;

            if (Math.Abs(totalChange) < 0.000001)
            {
                return Math.Abs(CurrentValue - TargetValue)
                    < 0.000001 ? 100 : 0;
            }

            double currentChange =
                CurrentValue - InitialValue;

            double progress =
                (currentChange / totalChange) * 100;

            return Math.Clamp(progress, 0, 100);
        }

        public void UpdateProgress(double currentValue)
        {
            CurrentValue = currentValue;

            IsCompleted =
                CalculateProgress() >= 100;
        }
    }
}
