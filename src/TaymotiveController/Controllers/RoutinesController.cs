
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

        // Obtener todos los usuarios
        public List<User> GetUsers()
        {
            return fileHandler.Read();
        }

        // Obtener todas las rutinas
        public List<Routine> GetRoutines()
        {
            List<User> users = GetUsers();

            return users
                .SelectMany(user => user.Routines)
                .ToList();
        }

        // Obtener cantidad total de rutinas
        public int GetTotalRoutines()
        {
            return GetRoutines().Count;
        }

        // Obtener rutinas de un usuario
        public List<Routine> GetRoutinesByUser(int userId)
        {
            User? user = GetUsers()
                .FirstOrDefault(u => u.Id == userId);

            return user?.Routines ?? new List<Routine>();
        }

        // Buscar rutina por ID
        public Routine? GetRoutineById(int routineId)
        {
            return GetRoutines()
                .FirstOrDefault(r => r.Id == routineId);
        }

        // Registrar y asignar una rutina a un usuario
        public bool RegisterRoutine(
            int userId,
            string name,
            string description,
            int duration,
            DateTime startDate,
            DateTime endDate)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(name) ||
                duration <= 0 ||
                endDate < startDate)
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
                description.Trim(),
                duration,
                startDate,
                endDate
            );

            user.Routines.Add(routine);

            fileHandler.Save(users);

            return true;
        }

        // Modificar una rutina existente
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
                endDate < startDate)
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
            routine.Description = description.Trim();
            routine.Duration = duration;
            routine.StartDate = startDate;
            routine.EndDate = endDate;

            fileHandler.Save(users);

            return true;
        }

        // Eliminar una rutina
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

        // Asignar un entrenador a una rutina existente
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

        // Obtener las rutinas de un entrenador
        public List<Routine> GetRoutinesByTrainer(
            int trainerId)
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

        // Obtener rutinas próximas a vencer
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

            DateTime limitDate =
                today.AddDays(daysBeforeExpiration);

            return GetRoutinesByTrainer(trainerId)
                .Where(routine =>
                    routine.EndDate.Date >= today &&
                    routine.EndDate.Date <= limitDate)
                .OrderBy(routine => routine.EndDate)
                .ToList();
        }
    }
}
