
using TaymotiveTests;
using TaymotiveController.Data;
using TaymotiveModels.Models;

DataGenerator.GenerateUsers();
DataGenerator.GenerateTrainers();

string filePath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Files",
    "users.json"
);

FileHandler<User> fileHandler =
    new FileHandler<User>(filePath);

List<User> users = fileHandler.Read();

Console.WriteLine("Usuarios registrados: " + users.Count);

int totalRoutines = 0;

foreach (User user in users)
{
    totalRoutines += user.Routines.Count;
}

Console.WriteLine("Total de rutinas: " + totalRoutines);

string trainersPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Files",
    "trainers.json"
);

FileHandler<Trainer> trainerHandler =
    new FileHandler<Trainer>(trainersPath);

List<Trainer> trainers = trainerHandler.Read();

Console.WriteLine("Entrenadores registrados: " + trainers.Count);
