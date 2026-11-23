
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class FitnessGoalsController
    {
        private readonly FileHandler<FitnessGoal> fileHandler;

        public FitnessGoalsController(string filePath)
        {
            fileHandler = new FileHandler<FitnessGoal>(filePath);
        }

        // Obtener todos los objetivos
        public List<FitnessGoal> GetGoals()
        {
            return fileHandler.Read();
        }

        // Obtener los objetivos de un usuario
        public List<FitnessGoal> GetGoalsByUser(int userId)
        {
            return fileHandler.Read()
                .Where(goal => goal.UserId == userId)
                .OrderBy(goal => goal.EndDate)
                .ToList();
        }

        // Buscar un objetivo por ID
        public FitnessGoal? GetGoalById(int id)
        {
            return fileHandler.Read()
                .FirstOrDefault(goal => goal.Id == id);
        }

        // Registrar un nuevo objetivo
        public bool RegisterGoal(
            int userId,
            string goalType,
            double initialValue,
            double targetValue,
            DateTime startDate,
            DateTime endDate)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(goalType) ||
                !double.IsFinite(initialValue) ||
                !double.IsFinite(targetValue) ||
                initialValue < 0 ||
                targetValue < 0 ||
                startDate.Date > endDate.Date)
            {
                return false;
            }

            List<FitnessGoal> goals = fileHandler.Read();

            int newId = goals.Count > 0
                ? goals.Max(goal => goal.Id) + 1
                : 1;

            FitnessGoal newGoal = new FitnessGoal(
                newId,
                userId,
                goalType.Trim(),
                initialValue,
                targetValue,
                startDate,
                endDate
            );

            newGoal.UpdateProgress(initialValue);

            goals.Add(newGoal);

            fileHandler.Save(goals);

            return true;
        }

        // Modificar un objetivo existente
        public bool UpdateGoal(
            int id,
            int userId,
            string goalType,
            double initialValue,
            double targetValue,
            DateTime startDate,
            DateTime endDate)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(goalType) ||
                !double.IsFinite(initialValue) ||
                !double.IsFinite(targetValue) ||
                initialValue < 0 ||
                targetValue < 0 ||
                startDate.Date > endDate.Date)
            {
                return false;
            }

            List<FitnessGoal> goals = fileHandler.Read();

            FitnessGoal? goal = goals.FirstOrDefault(
                item => item.Id == id
            );

            if (goal == null)
            {
                return false;
            }

            goal.UserId = userId;
            goal.GoalType = goalType.Trim();
            goal.InitialValue = initialValue;
            goal.TargetValue = targetValue;
            goal.StartDate = startDate;
            goal.EndDate = endDate;

            goal.UpdateProgress(goal.CurrentValue);

            fileHandler.Save(goals);

            return true;
        }

        // Actualizar el progreso físico
        public bool UpdateGoalProgress(
            int id,
            double currentValue)
        {
            if (!double.IsFinite(currentValue) ||
                currentValue < 0)
            {
                return false;
            }

            List<FitnessGoal> goals = fileHandler.Read();

            FitnessGoal? goal = goals.FirstOrDefault(
                item => item.Id == id
            );

            if (goal == null)
            {
                return false;
            }

            goal.UpdateProgress(currentValue);

            fileHandler.Save(goals);

            return true;
        }

        // Obtener porcentaje de progreso
        public double GetGoalProgress(int id)
        {
            FitnessGoal? goal = GetGoalById(id);

            if (goal == null)
            {
                return 0;
            }

            return goal.CalculateProgress();
        }

        // Eliminar un objetivo
        public bool DeleteGoal(int id)
        {
            List<FitnessGoal> goals = fileHandler.Read();

            FitnessGoal? goal = goals.FirstOrDefault(
                item => item.Id == id
            );

            if (goal == null)
            {
                return false;
            }

            goals.Remove(goal);

            fileHandler.Save(goals);

            return true;
        }

        // Actualizar automáticamente los objetivos de visitas
        public void UpdateAllVisitGoals(
            VisitsController visitsController)
        {
            List<FitnessGoal> goals = fileHandler.Read();

            List<Visit> visits =
                visitsController.GetVisits();

            bool changesMade = false;

            foreach (FitnessGoal goal in goals)
            {
                // Verificar si el objetivo es de visitas
                if (!goal.GoalType.Trim().Equals(
                    "Aumentar visitas",
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Contar las visitas del usuario
                // dentro del período del objetivo
                int totalVisits = visits.Count(
                    visit =>
                        visit.UserId == goal.UserId &&
                        visit.VisitDate.Date >=
                            goal.StartDate.Date &&
                        visit.VisitDate.Date <=
                            goal.EndDate.Date
                );

                // Actualizar el progreso del objetivo
                goal.UpdateProgress(totalVisits);

                changesMade = true;
            }

            // Guardar los cambios en JSON
            if (changesMade)
            {
                fileHandler.Save(goals);
            }
        }
    }
}
