
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
        private readonly MeasurementsController? measurementsController;

        private const int MeasurementIntervalDays = 30;
        private const int MeasurementNoticeDays = 7;

        // Constructor compatible con las vistas existentes
        public NotificationsController(
            RoutinesController routinesController,
            VisitsController visitsController,
            BadgesController badgesController)
            : this(
                routinesController,
                visitsController,
                badgesController,
                null)
        {
        }

        // Constructor con soporte para mediciones
        public NotificationsController(
            RoutinesController routinesController,
            VisitsController visitsController,
            BadgesController badgesController,
            MeasurementsController? measurementsController)
        {
            this.routinesController = routinesController;
            this.visitsController = visitsController;
            this.badgesController = badgesController;
            this.measurementsController = measurementsController;
        }

        // Obtener timeline de notificaciones del usuario
        public List<Notification> GetUserTimeline(int userId)
        {
            List<Notification> notifications =
                new List<Notification>();

            if (userId <= 0)
            {
                return notifications;
            }

            AddRoutineNotifications(userId, notifications);
            AddBadgeNotifications(userId, notifications);
            AddRestNotifications(userId, notifications);

            return notifications
                .OrderByDescending(item => item.Date)
                .ThenBy(item => item.Message)
                .ToList();
        }

        // Compatibilidad con otras vistas
        public List<string> GetUserNotifications(int userId)
        {
            return GetUserTimeline(userId)
                .Select(item => item.Message)
                .ToList();
        }

        // Obtener notificaciones del entrenador
        public List<Notification> GetTrainerTimeline(int trainerId)
        {
            List<Notification> notifications =
                new List<Notification>();

            if (trainerId <= 0)
            {
                return notifications;
            }

            AddExpiringRoutineNotifications(
                trainerId,
                notifications
            );

            AddUpcomingMeasurementNotifications(
                trainerId,
                notifications
            );

            return notifications
                .OrderBy(item => item.Date)
                .ThenBy(item => item.Message)
                .ToList();
        }

        // Notificaciones de rutinas asignadas al usuario
        private void AddRoutineNotifications(
            int userId,
            List<Notification> notifications)
        {
            List<Routine> routines =
                routinesController.GetRoutinesByUser(userId);

            foreach (Routine routine in routines)
            {
                notifications.Add(
                    new Notification(
                        userId,
                        "Nueva rutina asignada: " +
                            routine.Name,
                        "Rutina",
                        routine.StartDate
                    )
                );
            }
        }

        // Insignias positivas y negativas
        private void AddBadgeNotifications(
            int userId,
            List<Notification> notifications)
        {
            List<Badge> badges =
                badgesController.GetBadgesByUser(userId);

            foreach (Badge badge in badges)
            {
                string type = badge.BadgeType;

                string message;

                if (type.Equals(
                    "Negativa",
                    StringComparison.OrdinalIgnoreCase))
                {
                    message =
                        "Aviso de mejora: " +
                        badge.Name;
                }
                else
                {
                    message =
                        "Has ganado la insignia: " +
                        badge.Name;
                }

                notifications.Add(
                    new Notification(
                        userId,
                        message,
                        type,
                        badge.AwardedDate
                    )
                );
            }
        }

        // Aviso de descanso después de 5 visitas
        private void AddRestNotifications(
            int userId,
            List<Notification> notifications)
        {
            DateTime today = DateTime.Today;

            int daysSinceMonday =
                ((int)today.DayOfWeek + 6) % 7;

            DateTime startOfWeek =
                today.AddDays(-daysSinceMonday);

            DateTime endOfWeek =
                startOfWeek.AddDays(7);

            List<Visit> weeklyVisits =
                visitsController.GetVisits()
                    .Where(visit =>
                        visit.UserId == userId &&
                        visit.VisitDate.Date >= startOfWeek &&
                        visit.VisitDate.Date < endOfWeek
                    )
                    .OrderBy(visit => visit.VisitDate)
                    .ToList();

            if (weeklyVisits.Count >= 5)
            {
                DateTime notificationDate =
                    weeklyVisits[4].VisitDate;

                notifications.Add(
                    new Notification(
                        userId,
                        "Descanso recomendado: " +
                        "has registrado al menos " +
                        "5 visitas esta semana.",
                        "Descanso",
                        notificationDate
                    )
                );
            }
        }

        // Avisos de rutinas próximas a vencer
        private void AddExpiringRoutineNotifications(
            int trainerId,
            List<Notification> notifications)
        {
            List<Routine> expiringRoutines =
                routinesController
                    .GetExpiringRoutinesByTrainer(
                        trainerId,
                        7
                    );

            List<User> users =
                routinesController.GetUsers();

            foreach (Routine routine in expiringRoutines)
            {
                User? user = users.FirstOrDefault(
                    item => item.Routines != null &&
                        item.Routines.Any(
                            assignedRoutine =>
                                assignedRoutine.Id ==
                                routine.Id
                        )
                );

                string userName = user == null
                    ? "Usuario no identificado"
                    : user.Name;

                string message =
                    "La rutina " + routine.Name +
                    " de " + userName +
                    " vence el " +
                    routine.EndDate.ToString("dd/MM/yyyy") +
                    ". Se recomienda preparar una nueva rutina.";

                notifications.Add(
                    new Notification(
                        trainerId,
                        message,
                        "Vencimiento",
                        routine.EndDate
                    )
                );
            }
        }

        // Avisos de próximas mediciones físicas
        private void AddUpcomingMeasurementNotifications(
            int trainerId,
            List<Notification> notifications)
        {
            if (measurementsController == null)
            {
                return;
            }

            List<User> users =
                routinesController.GetUsers();

            List<Measurement> measurements =
                measurementsController.GetMeasurements();

            DateTime today = DateTime.Today;

            foreach (User user in users)
            {
                // Solo usuarios con rutinas asignadas
                // al entrenador seleccionado
                bool assignedToTrainer =
                    user.Routines != null &&
                    user.Routines.Any(
                        routine =>
                            routine.TrainerId == trainerId
                    );

                if (!assignedToTrainer)
                {
                    continue;
                }

                Measurement? latestMeasurement =
                    measurements
                        .Where(item =>
                            item.UserId == user.Id &&
                            item.MeasurementDate.Date <= today
                        )
                        .OrderByDescending(item =>
                            item.MeasurementDate)
                        .FirstOrDefault();

                // Si no tiene mediciones, avisar
                // que debe programarse la primera
                if (latestMeasurement == null)
                {
                    notifications.Add(
                        new Notification(
                            trainerId,
                            "El usuario " + user.Name +
                            " no tiene mediciones registradas. " +
                            "Se recomienda programar su primera medición.",
                            "Medición",
                            today
                        )
                    );

                    continue;
                }

                DateTime nextMeasurement =
                    latestMeasurement.MeasurementDate.Date
                        .AddDays(MeasurementIntervalDays);

                DateTime noticeStart =
                    nextMeasurement.AddDays(
                        -MeasurementNoticeDays
                    );

                // Mostrar desde 7 días antes
                // y mantener el aviso si está vencida
                if (today >= noticeStart)
                {
                    string message;

                    if (today > nextMeasurement)
                    {
                        message =
                            "La medición de " + user.Name +
                            " está pendiente desde el " +
                            nextMeasurement.ToString("dd/MM/yyyy") +
                            ". Se recomienda actualizar su progreso.";
                    }
                    else
                    {
                        message =
                            "La próxima medición de " +
                            user.Name +
                            " corresponde al " +
                            nextMeasurement.ToString("dd/MM/yyyy") +
                            ". Se recomienda programarla.";
                    }

                    notifications.Add(
                        new Notification(
                            trainerId,
                            message,
                            "Medición",
                            nextMeasurement
                        )
                    );
                }
            }
        }
    }
}
