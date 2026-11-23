
using System;
using TaymotiveModels.Models;
using Xunit;

namespace TaymotiveTests
{
    public class FitnessGoalTests
    {
        // Verificar el progreso de un objetivo
        [Fact]
        public void CalculateProgress_Returns50Percent()
        {
            FitnessGoal goal = new FitnessGoal(
                1,
                1,
                "Ganar musculatura",
                60,
                70,
                DateTime.Today,
                DateTime.Today.AddDays(30)
            );

            goal.UpdateProgress(65);

            double result = goal.CalculateProgress();

            Assert.Equal(50, result);
        }

        // Verificar un objetivo completado
        [Fact]
        public void UpdateProgress_CompletesGoal()
        {
            FitnessGoal goal = new FitnessGoal(
                2,
                1,
                "Ganar musculatura",
                60,
                70,
                DateTime.Today,
                DateTime.Today.AddDays(30)
            );

            goal.UpdateProgress(70);

            Assert.True(goal.IsCompleted);
            Assert.Equal(100, goal.CalculateProgress());
        }

        // Verificar que el progreso no supere 100%
        [Fact]
        public void CalculateProgress_DoesNotExceed100Percent()
        {
            FitnessGoal goal = new FitnessGoal(
                3,
                1,
                "Ganar musculatura",
                60,
                70,
                DateTime.Today,
                DateTime.Today.AddDays(30)
            );

            goal.UpdateProgress(75);

            Assert.Equal(100, goal.CalculateProgress());
        }
    }
}
