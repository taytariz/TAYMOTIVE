
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class RoutineProgressController
    {
        private readonly FileHandler<RoutineProgress> progressFile;

        public RoutineProgressController(string filePath)
        {
            progressFile = new FileHandler<RoutineProgress>(
                filePath
            );
        }

        // Obtener todos los registros de progreso
        public List<RoutineProgress> GetProgress()
        {
            return progressFile.Read();
        }

        // Obtener el progreso de un usuario
        public List<RoutineProgress> GetProgressByUser(int userId)
        {
            return progressFile.Read()
                .Where(progress => progress.UserId == userId)
                .OrderByDescending(
                    progress => progress.ProgressDate
                )
                .ThenByDescending(progress => progress.Id)
                .ToList();
        }

        // Obtener el progreso de una rutina
        public List<RoutineProgress> GetProgressByRoutine(
            int userId,
            int routineId)
        {
            return progressFile.Read()
                .Where(progress =>
                    progress.UserId == userId &&
                    progress.RoutineId == routineId
                )
                .OrderBy(progress => progress.ProgressDate)
                .ThenBy(progress => progress.Id)
                .ToList();
        }

        // Registrar un nuevo progreso
        public void AddProgress(RoutineProgress progress)
        {
            if (progress == null)
            {
                throw new ArgumentNullException(
                    nameof(progress)
                );
            }

            if (progress.UserId <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un usuario válido."
                );
            }

            if (progress.RoutineId <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar una rutina válida."
                );
            }

            if (string.IsNullOrWhiteSpace(
                progress.ExerciseName))
            {
                throw new ArgumentException(
                    "Debe indicar el nombre del ejercicio."
                );
            }

            if (double.IsNaN(progress.Weight) ||
                double.IsInfinity(progress.Weight) ||
                progress.Weight < 0)
            {
                throw new ArgumentException(
                    "El peso debe ser un número válido " +
                    "mayor o igual a cero."
                );
            }

            if (progress.ProgressDate == default ||
                progress.ProgressDate.Date > DateTime.Today)
            {
                throw new ArgumentException(
                    "Debe indicar una fecha válida " +
                    "que no sea futura."
                );
            }

            List<RoutineProgress> records =
                progressFile.Read();

            progress.Id = records.Count == 0
                ? 1
                : records.Max(item => item.Id) + 1;

            progress.ExerciseName =
                progress.ExerciseName.Trim();

            records.Add(progress);

            progressFile.Save(records);
        }

        // Eliminar un registro de progreso
        public bool DeleteProgress(int progressId, int userId)
        {
            List<RoutineProgress> records =
                progressFile.Read();

            RoutineProgress? progress =
                records.FirstOrDefault(item =>
                    item.Id == progressId &&
                    item.UserId == userId
                );

            if (progress == null)
            {
                return false;
            }

            records.Remove(progress);
            progressFile.Save(records);

            return true;
        }
    }
}
