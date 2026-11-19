
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class TrainersController
    {
        private readonly FileHandler<Trainer> fileHandler;

        public TrainersController(string filePath)
        {
            fileHandler = new FileHandler<Trainer>(filePath);
        }

        // Obtener todos los entrenadores
        public List<Trainer> GetTrainers()
        {
            return fileHandler.Read();
        }

        // Obtener cantidad total de entrenadores
        public int GetTotalTrainers()
        {
            return GetTrainers().Count;
        }

        // Buscar entrenador por ID
        public Trainer? GetTrainerById(int id)
        {
            return GetTrainers().FirstOrDefault(
                trainer => trainer.Id == id
            );
        }

        // Registrar un nuevo entrenador
        public bool RegisterTrainer(
            string name,
            int age,
            double weight,
            double height,
            string specialty,
            bool isPersonalTrainer)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(specialty) ||
                age <= 0 ||
                weight <= 0 ||
                height <= 0)
            {
                return false;
            }

            List<Trainer> trainers = GetTrainers();

            int newId = trainers.Count == 0
                ? 1
                : trainers.Max(trainer => trainer.Id) + 1;

            Trainer newTrainer = new Trainer(
                newId,
                name.Trim(),
                age,
                weight,
                height,
                specialty.Trim(),
                isPersonalTrainer
            );

            trainers.Add(newTrainer);

            fileHandler.Save(trainers);

            return true;
        }

        // Modificar un entrenador existente
        public bool UpdateTrainer(
            int id,
            string name,
            int age,
            double weight,
            double height,
            string specialty,
            bool isPersonalTrainer)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(specialty) ||
                age <= 0 ||
                weight <= 0 ||
                height <= 0)
            {
                return false;
            }

            List<Trainer> trainers = GetTrainers();

            Trainer? trainerToUpdate = trainers.FirstOrDefault(
                trainer => trainer.Id == id
            );

            if (trainerToUpdate == null)
            {
                return false;
            }

            trainerToUpdate.Name = name.Trim();
            trainerToUpdate.Age = age;
            trainerToUpdate.Weight = weight;
            trainerToUpdate.Height = height;
            trainerToUpdate.Specialty = specialty.Trim();
            trainerToUpdate.IsPersonalTrainer = isPersonalTrainer;

            fileHandler.Save(trainers);

            return true;
        }

        // Eliminar un entrenador existente
        public bool DeleteTrainer(int id)
        {
            List<Trainer> trainers = GetTrainers();

            Trainer? trainerToDelete = trainers.FirstOrDefault(
                trainer => trainer.Id == id
            );

            if (trainerToDelete == null)
            {
                return false;
            }

            trainers.Remove(trainerToDelete);

            fileHandler.Save(trainers);

            return true;
        }
    }
}
