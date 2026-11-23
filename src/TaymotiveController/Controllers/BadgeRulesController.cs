
using System;
using System.Linq;
using System.Collections.Generic;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class BadgeRulesController
    {
        private readonly BadgesController badgesController;
        private readonly VisitsController visitsController;
        private readonly FitnessGoalsController goalsController;
        private readonly UsersController? usersController;
        private readonly MeasurementsController? measurementsController;

        public BadgeRulesController(
            BadgesController badgesController,
            VisitsController visitsController,
            FitnessGoalsController goalsController,
            UsersController? usersController = null,
            MeasurementsController? measurementsController = null)
        {
            this.badgesController = badgesController;
            this.visitsController = visitsController;
            this.goalsController = goalsController;
            this.usersController = usersController;
            this.measurementsController = measurementsController;
        }

        // Evaluar las insignias de un usuario
        public void EvaluateUserBadges(int userId)
        {
            if (userId <= 0)
            {
                return;
            }

            // Excluir cuentas administrativas
            if (usersController != null)
            {
                User? user =
                    usersController.GetUserById(userId);

                if (user == null)
                {
                    return;
                }

                if (user.IsAdmin)
                {
                    RemoveBadgeIfExists(
                        userId,
                        "Baja asistencia"
                    );

                    RemoveBadgeIfExists(
                        userId,
                        "Progreso desactualizado"
                    );

                    return;
                }
            }

            EvaluateVisitBadges(userId);
            EvaluateGoalBadges(userId);
            EvaluateLowAttendanceBadge(userId);
            EvaluateMeasurementBadge(userId);
        }

        // Evaluar insignias positivas por visitas
        private void EvaluateVisitBadges(int userId)
        {
            int totalVisits = visitsController.GetVisits()
                .Count(visit => visit.UserId == userId);

            if (totalVisits >= 1)
            {
                badgesController.AwardBadge(
                    userId,
                    "Primera visita",
                    "Registró su primera visita al gimnasio.",
                    "Positiva"
                );
            }

            if (totalVisits >= 5)
            {
                badgesController.AwardBadge(
                    userId,
                    "Constancia",
                    "Acumuló al menos 5 visitas al gimnasio.",
                    "Positiva"
                );
            }
        }

        // Evaluar insignias por objetivos físicos
        private void EvaluateGoalBadges(int userId)
        {
            List<FitnessGoal> goals =
                goalsController.GetGoalsByUser(userId);

            bool completedGoal = goals.Any(
                goal => goal.IsCompleted
            );

            if (completedGoal)
            {
                badgesController.AwardBadge(
                    userId,
                    "Meta cumplida",
                    "Completó al menos un objetivo físico.",
                    "Positiva"
                );
            }

            bool failedGoal = goals.Any(
                goal =>
                    goal.EndDate.Date < DateTime.Today &&
                    !goal.IsCompleted
            );

            if (failedGoal)
            {
                badgesController.AwardBadge(
                    userId,
                    "Meta incumplida",
                    "Tiene al menos un objetivo vencido sin completar.",
                    "Negativa"
                );
            }
            else
            {
                RemoveBadgeIfExists(
                    userId,
                    "Meta incumplida"
                );
            }
        }

        // Evaluar usuarios con menor asistencia
        private void EvaluateLowAttendanceBadge(int userId)
        {
            if (usersController == null)
            {
                return;
            }

            List<User> users = usersController.GetUsers()
                .Where(user => !user.IsAdmin)
                .ToList();

            List<Visit> visits =
                visitsController.GetVisits();

            var lowestAttendanceUsers = users
                .Select(user => new
                {
                    UserId = user.Id,
                    TotalVisits = visits.Count(
                        visit => visit.UserId == user.Id
                    )
                })
                .OrderBy(item => item.TotalVisits)
                .ThenBy(item => item.UserId)
                .Take(10)
                .Select(item => item.UserId)
                .ToList();

            bool hasLowAttendance =
                lowestAttendanceUsers.Contains(userId);

            if (hasLowAttendance)
            {
                badgesController.AwardBadge(
                    userId,
                    "Baja asistencia",
                    "Se encuentra entre los 10 usuarios " +
                    "con menos visitas al gimnasio.",
                    "Negativa"
                );
            }
            else
            {
                RemoveBadgeIfExists(
                    userId,
                    "Baja asistencia"
                );
            }
        }

        // Evaluar falta de actualización de mediciones
        private void EvaluateMeasurementBadge(int userId)
        {
            if (measurementsController == null)
            {
                return;
            }

            List<Measurement> measurements =
                measurementsController.GetMeasurementsByUser(
                    userId
                );

            // No penalizar usuarios sin mediciones
            if (measurements.Count == 0)
            {
                RemoveBadgeIfExists(
                    userId,
                    "Progreso desactualizado"
                );

                return;
            }

            DateTime lastMeasurementDate =
                measurements[0].MeasurementDate.Date;

            // La actualización se espera cada 30 días
            DateTime nextMeasurementDate =
                lastMeasurementDate.AddDays(30);

            if (DateTime.Today > nextMeasurementDate)
            {
                badgesController.AwardBadge(
                    userId,
                    "Progreso desactualizado",
                    "No ha actualizado sus mediciones " +
                    "físicas dentro del período de 30 días.",
                    "Negativa"
                );
            }
            else
            {
                RemoveBadgeIfExists(
                    userId,
                    "Progreso desactualizado"
                );
            }
        }

        // Eliminar insignias que ya no corresponden
        private void RemoveBadgeIfExists(
            int userId,
            string badgeName)
        {
            List<Badge> badges = badgesController
                .GetBadgesByUser(userId)
                .Where(badge =>
                    badge.Name.Equals(
                        badgeName,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                .ToList();

            foreach (Badge badge in badges)
            {
                badgesController.DeleteBadge(badge.Id);
            }
        }
    }
}
