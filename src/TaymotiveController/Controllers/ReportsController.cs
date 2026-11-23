
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class ReportsController
    {
        private readonly FileHandler<User> usersFile;
        private readonly FileHandler<Trainer> trainersFile;
        private readonly FileHandler<Measurement> measurementsFile;
        private readonly FileHandler<Visit> visitsFile;

        public ReportsController(
            string usersPath,
            string trainersPath,
            string measurementsPath,
            string visitsPath)
        {
            usersFile = new FileHandler<User>(usersPath);
            trainersFile = new FileHandler<Trainer>(trainersPath);
            measurementsFile = new FileHandler<Measurement>(
                measurementsPath
            );
            visitsFile = new FileHandler<Visit>(visitsPath);
        }

        // Obtener resumen general del gimnasio
        public GeneralReport GetGeneralReport()
        {
            List<User> users = usersFile.Read();
            List<Trainer> trainers = trainersFile.Read();
            List<Measurement> measurements = measurementsFile.Read();
            List<Visit> visits = visitsFile.Read();

            return new GeneralReport
            {
                TotalUsers = users.Count,
                TotalTrainers = trainers.Count,
                TotalRoutines = users.Sum(
                    user => user.Routines?.Count ?? 0
                ),
                TotalMeasurements = measurements.Count,
                TotalVisits = visits.Count
            };
        }

        // Obtener los 10 usuarios básicos con más visitas
        public List<UserVisitReport> GetTopVisitors(
            DateTime startDate,
            DateTime endDate)
        {
            List<User> users = usersFile.Read()
                .Where(user => !user.IsAdmin)
                .ToList();

            List<Visit> visits = GetVisitsByDate(
                startDate,
                endDate
            );

            return users
                .Select(user => new UserVisitReport
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    TotalVisits = visits.Count(
                        visit => visit.UserId == user.Id
                    )
                })
                .OrderByDescending(report => report.TotalVisits)
                .ThenBy(report => report.UserName)
                .Take(10)
                .ToList();
        }

        // Obtener los 10 usuarios básicos con menos visitas
        public List<UserVisitReport> GetLeastVisitors(
            DateTime startDate,
            DateTime endDate)
        {
            List<User> users = usersFile.Read()
                .Where(user => !user.IsAdmin)
                .ToList();

            List<Visit> visits = GetVisitsByDate(
                startDate,
                endDate
            );

            return users
                .Select(user => new UserVisitReport
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    TotalVisits = visits.Count(
                        visit => visit.UserId == user.Id
                    )
                })
                .OrderBy(report => report.TotalVisits)
                .ThenBy(report => report.UserName)
                .Take(10)
                .ToList();
        }

        // Obtener visitas por tipo de clase
        public List<ClassReport> GetVisitsByClass(
            DateTime startDate,
            DateTime endDate)
        {
            List<Visit> visits = GetVisitsByDate(
                startDate,
                endDate
            );

            return visits
                .GroupBy(visit => visit.ClassName)
                .Select(group => new ClassReport
                {
                    ClassName = group.Key,
                    TotalVisits = group.Count()
                })
                .OrderByDescending(report => report.TotalVisits)
                .ToList();
        }

        // Obtener historial de mediciones de un usuario
        public List<Measurement> GetUserMeasurements(
            int userId,
            DateTime startDate,
            DateTime endDate)
        {
            return measurementsFile.Read()
                .Where(measurement =>
                    measurement.UserId == userId &&
                    measurement.MeasurementDate.Date >=
                        startDate.Date &&
                    measurement.MeasurementDate.Date <=
                        endDate.Date
                )
                .OrderBy(measurement =>
                    measurement.MeasurementDate
                )
                .ToList();
        }

        // Calcular cambio de peso de un usuario
        public double GetWeightChange(
            int userId,
            DateTime startDate,
            DateTime endDate)
        {
            List<Measurement> measurements =
                GetUserMeasurements(
                    userId,
                    startDate,
                    endDate
                );

            if (measurements.Count < 2)
            {
                return 0;
            }

            double initialWeight = measurements.First().Weight;
            double finalWeight = measurements.Last().Weight;

            return finalWeight - initialWeight;
        }

        // Obtener las últimas cinco rutinas
        public List<Routine> GetLastFiveRoutines(int userId)
        {
            User? user = usersFile.Read()
                .FirstOrDefault(u => u.Id == userId);

            if (user == null || user.Routines == null)
            {
                return new List<Routine>();
            }

            return user.Routines
                .OrderByDescending(routine => routine.StartDate)
                .Take(5)
                .ToList();
        }

        // Obtener visitas dentro de un rango de fechas
        private List<Visit> GetVisitsByDate(
            DateTime startDate,
            DateTime endDate)
        {
            if (startDate.Date > endDate.Date)
            {
                return new List<Visit>();
            }

            return visitsFile.Read()
                .Where(visit =>
                    visit.VisitDate.Date >= startDate.Date &&
                    visit.VisitDate.Date <= endDate.Date
                )
                .ToList();
        }
    }

    // Resumen general
    public class GeneralReport
    {
        public int TotalUsers { get; set; }
        public int TotalTrainers { get; set; }
        public int TotalRoutines { get; set; }
        public int TotalMeasurements { get; set; }
        public int TotalVisits { get; set; }
    }

    // Estadísticas de visitas por usuario
    public class UserVisitReport
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalVisits { get; set; }
    }

    // Estadísticas de clases
    public class ClassReport
    {
        public string ClassName { get; set; } = string.Empty;
        public int TotalVisits { get; set; }
    }
}
