
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

        public BadgeRulesController(
            BadgesController badgesController,
            VisitsController visitsController,
            FitnessGoalsController goalsController,
            UsersController? usersController = null)
        {
            this.badgesController = badgesController;
            this.visitsController = visitsController;
            this.goalsController = goalsController;
            this.usersController = usersController;
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
                    // Retirar insignia negativa anterior
                    RemoveBadgeIfExists(
                        userId,
                        "Baja asistencia"
                    );

                    return;
                }
            }

            EvaluateVisitBadges(userId);
            EvaluateGoalBadges(userId);
            EvaluateLowAttendanceBadge(userId);
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

            // Verificar objetivos completados
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

            // Verificar objetivos vencidos
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

            var visits = visitsController.GetVisits();

            // Obtener los 10 usuarios normales
            // con menor cantidad de visitas
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

        // Eliminar insignias negativas que
        // ya no corresponden al usuario
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
