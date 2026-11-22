
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
            if (userId <= 0 ||
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
