
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class RoutinesController
    {
        private readonly FileHandler<User> fileHandler;

        public RoutinesController(string filePath)
        {
            fileHandler = new FileHandler<User>(filePath);
        }

        public List<User> GetUsers()
        {
            return fileHandler.Read();
        }

        public List<Routine> GetRoutines()
        {
            return GetUsers()
                .SelectMany(user => user.Routines)
                .ToList();
        }

        public int GetTotalRoutines()
        {
            return GetRoutines().Count;
        }

        public List<Routine> GetRoutinesByUser(int userId)
        {
            User? user = GetUsers()
                .FirstOrDefault(u => u.Id == userId);

            return user?.Routines ?? new List<Routine>();
        }

        public Routine? GetRoutineById(int routineId)
        {
            return GetRoutines()
                .FirstOrDefault(r => r.Id == routineId);
        }

        // Constructor de registro original.
        // Se conserva para mantener compatibilidad.
        public bool RegisterRoutine(
            int userId,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate)
        {
            return RegisterRoutine(
                userId,
                name,
                description,
                duration,
                startDate,
                endDate,
                0,
                0
            );
        }

        // Registro con series y repeticiones.
        public bool RegisterRoutine(
            int userId,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate,
            int sets,
            int repetitions)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(name) ||
                duration <= 0 ||
                endDate.Date < startDate.Date ||
                sets < 0 ||
                repetitions < 0 ||
                (sets == 0 && repetitions > 0) ||
                (sets > 0 && repetitions == 0))
            {
                return false;
            }

            List<User> users = GetUsers();

            User? user = users
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            int newId = users
                .SelectMany(u => u.Routines)
                .Select(r => r.Id)
                .DefaultIfEmpty(0)
                .Max() + 1;

            Routine routine = new Routine(
                newId,
                name.Trim(),
                description?.Trim() ?? string.Empty,
                duration,
                startDate,
                endDate,
                0,
                sets,
                repetitions
            );

            user.Routines.Add(routine);
            fileHandler.Save(users);

            return true;
        }

        // Edición original.
        public bool UpdateRoutine(
            int routineId,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                duration <= 0 ||
                endDate.Date < startDate.Date)
            {
                return false;
            }

            List<User> users = GetUsers();

            Routine? routine = users
                .SelectMany(u => u.Routines)
                .FirstOrDefault(r => r.Id == routineId);

            if (routine == null)
            {
                return false;
            }

            routine.Name = name.Trim();
            routine.Description = description?.Trim() ?? string.Empty;
            routine.Duration = duration;
            routine.StartDate = startDate;
            routine.EndDate = endDate;

            fileHandler.Save(users);

            return true;
        }

        // Edición con series y repeticiones.
        public bool UpdateRoutine(
            int routineId,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate,
            int sets,
            int repetitions)
        {
            if (sets < 0 ||
                repetitions < 0 ||
                (sets == 0 && repetitions > 0) ||
                (sets > 0 && repetitions == 0))
            {
                return false;
            }

            List<User> users = GetUsers();

            Routine? routine = users
                .SelectMany(u => u.Routines)
                .FirstOrDefault(r => r.Id == routineId);

            if (routine == null ||
                string.IsNullOrWhiteSpace(name) ||
                duration <= 0 ||
                endDate.Date < startDate.Date)
            {
                return false;
            }

            routine.Name = name.Trim();
            routine.Description = description?.Trim() ?? string.Empty;
            routine.Duration = duration;
            routine.StartDate = startDate;
            routine.EndDate = endDate;
            routine.Sets = sets;
            routine.Repetitions = repetitions;

            fileHandler.Save(users);

            return true;
        }

        public bool DeleteRoutine(int routineId)
        {
            List<User> users = GetUsers();

            List<Routine>? userRoutines = users
                .Select(user => user.Routines)
                .FirstOrDefault(routines =>
                    routines.Any(routine =>
                        routine.Id == routineId));

            if (userRoutines == null)
            {
                return false;
            }

            Routine? routineToDelete = userRoutines
                .FirstOrDefault(routine =>
                    routine.Id == routineId);

            if (routineToDelete == null)
            {
                return false;
            }

            userRoutines.Remove(routineToDelete);
            fileHandler.Save(users);

            return true;
        }

        public bool AssignTrainerToRoutine(
            int routineId,
            int trainerId)
        {
            if (routineId <= 0 || trainerId <= 0)
            {
                return false;
            }

            List<User> users = GetUsers();

            Routine? routine = users
                .SelectMany(user => user.Routines)
                .FirstOrDefault(r => r.Id == routineId);

            if (routine == null)
            {
                return false;
            }

            routine.TrainerId = trainerId;
            fileHandler.Save(users);

            return true;
        }

        public List<Routine> GetRoutinesByTrainer(int trainerId)
        {
            if (trainerId <= 0)
            {
                return new List<Routine>();
            }

            return GetRoutines()
                .Where(routine =>
                    routine.TrainerId == trainerId)
                .OrderBy(routine => routine.EndDate)
                .ToList();
        }

        public List<Routine> GetExpiringRoutinesByTrainer(
            int trainerId,
            int daysBeforeExpiration = 7)
        {
            if (trainerId <= 0 ||
                daysBeforeExpiration < 0)
            {
                return new List<Routine>();
            }

            DateTime today = DateTime.Today;
            DateTime limitDate = today.AddDays(daysBeforeExpiration);

            return GetRoutinesByTrainer(trainerId)
                .Where(routine =>
                    routine.EndDate.Date >= today &&
                    routine.EndDate.Date <= limitDate)
                .OrderBy(routine => routine.EndDate)
                .ToList();
        }
    }
}
