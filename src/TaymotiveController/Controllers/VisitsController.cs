
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class VisitsController
    {
        private readonly FileHandler<Visit> fileHandler;

        public VisitsController(string filePath)
        {
            fileHandler = new FileHandler<Visit>(filePath);
        }

        // Obtener todas las visitas
        public List<Visit> GetVisits()
        {
            return fileHandler.Read();
        }

        // Buscar una visita por ID
        public Visit? GetVisitById(int id)
        {
            List<Visit> visits = fileHandler.Read();

            return visits.FirstOrDefault(
                visit => visit.Id == id
            );
        }

        // Obtener las visitas de un usuario
        public List<Visit> GetVisitsByUser(int userId)
        {
            if (userId <= 0)
            {
                return new List<Visit>();
            }

            List<Visit> visits = fileHandler.Read();

            return visits
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.VisitDate)
                .ToList();
        }

        // Contar todas las visitas de un usuario
        public int CountVisitsByUser(int userId)
        {
            if (userId <= 0)
            {
                return 0;
            }

            List<Visit> visits = fileHandler.Read();

            return visits.Count(
                v => v.UserId == userId
            );
        }

        // Contar visitas entre dos fechas
        public int CountVisitsByDateRange(
            int userId,
            DateTime startDate,
            DateTime endDate)
        {
            if (userId <= 0 ||
                startDate.Date > endDate.Date)
            {
                return 0;
            }

            List<Visit> visits = fileHandler.Read();

            return visits.Count(v =>
                v.UserId == userId &&
                v.VisitDate.Date >= startDate.Date &&
                v.VisitDate.Date <= endDate.Date
            );
        }

        // Comprobar si existe una visita equivalente
        private static bool VisitExists(
            List<Visit> visits,
            int userId,
            DateTime visitDate,
            string className,
            int excludedId = 0)
        {
            return visits.Any(v =>
                v.Id != excludedId &&
                v.UserId == userId &&
                v.VisitDate.Date == visitDate.Date &&
                string.Equals(
                    v.ClassName?.Trim(),
                    className.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }

        // Registrar una nueva visita
        public bool RegisterVisit(
            int userId,
            DateTime visitDate,
            string className)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(className))
            {
                return false;
            }

            List<Visit> visits = fileHandler.Read();

            // Evitar visitas duplicadas
            if (VisitExists(
                visits,
                userId,
                visitDate,
                className))
            {
                return false;
            }

            int newId = visits.Count > 0
                ? visits.Max(v => v.Id) + 1
                : 1;

            Visit newVisit = new Visit(
                newId,
                userId,
                visitDate,
                className.Trim()
            );

            visits.Add(newVisit);

            fileHandler.Save(visits);

            return true;
        }

        // Modificar una visita existente
        public bool UpdateVisit(
            int id,
            int userId,
            DateTime visitDate,
            string className)
        {
            if (id <= 0 ||
                userId <= 0 ||
                string.IsNullOrWhiteSpace(className))
            {
                return false;
            }

            List<Visit> visits = fileHandler.Read();

            Visit? visit = visits.FirstOrDefault(
                v => v.Id == id
            );

            if (visit == null)
            {
                return false;
            }

            // Evitar duplicados al modificar
            if (VisitExists(
                visits,
                userId,
                visitDate,
                className,
                id))
            {
                return false;
            }

            visit.UserId = userId;
            visit.VisitDate = visitDate;
            visit.ClassName = className.Trim();

            fileHandler.Save(visits);

            return true;
        }

        // Eliminar una visita
        public bool DeleteVisit(int id)
        {
            List<Visit> visits = fileHandler.Read();

            Visit? visit = visits.FirstOrDefault(
                v => v.Id == id
            );

            if (visit == null)
            {
                return false;
            }

            visits.Remove(visit);

            fileHandler.Save(visits);

            return true;
        }
    }
}
