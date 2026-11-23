
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class NotificationsController
    {
        private readonly RoutinesController routinesController;
        private readonly VisitsController visitsController;
        private readonly BadgesController badgesController;

        public NotificationsController(
            RoutinesController routinesController,
            VisitsController visitsController,
            BadgesController badgesController)
        {
            this.routinesController = routinesController;
            this.visitsController = visitsController;
            this.badgesController = badgesController;
        }

        // Obtener las notificaciones de un usuario
        public List<string> GetUserNotifications(int userId)
        {
            List<string> notifications =
                new List<string>();

            if (userId <= 0)
            {
                return notifications;
            }

            GetRoutineNotifications(
                userId,
                notifications
            );

            GetBadgeNotifications(
                userId,
                notifications
            );

            GetRestNotifications(
                userId,
                notifications
            );

            return notifications;
        }

        // Notificaciones de rutinas asignadas
        private void GetRoutineNotifications(
            int userId,
            List<string> notifications)
        {
            List<Routine> routines =
                routinesController.GetRoutinesByUser(userId);

            foreach (Routine routine in routines)
            {
                notifications.Add(
                    "Rutina asignada: " + routine.Name
                );
            }
        }

        // Notificaciones de insignias obtenidas
        private void GetBadgeNotifications(
            int userId,
            List<string> notifications)
        {
            var badges =
                badgesController.GetBadgesByUser(userId);

            foreach (var badge in badges)
            {
                notifications.Add(
                    "Insignia obtenida: " + badge.Name
                );
            }
        }

        // Recomendar descanso después de
        // cinco visitas durante una semana
        private void GetRestNotifications(
            int userId,
            List<string> notifications)
        {
            DateTime today = DateTime.Today;

            int daysSinceMonday =
                ((int)today.DayOfWeek + 6) % 7;

            DateTime startOfWeek =
                today.AddDays(-daysSinceMonday);

            DateTime endOfWeek =
                startOfWeek.AddDays(7);

            int weeklyVisits = visitsController
                .GetVisits()
                .Count(visit =>
                    visit.UserId == userId &&
                    visit.VisitDate.Date >= startOfWeek &&
                    visit.VisitDate.Date < endOfWeek
                );

            if (weeklyVisits >= 5)
            {
                notifications.Add(
                    "Descanso recomendado: " +
                    "registraste al menos 5 visitas " +
                    "durante esta semana."
                );
            }
        }
    }
}
