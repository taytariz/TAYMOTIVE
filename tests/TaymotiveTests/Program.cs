
using TaymotiveTests;
using TaymotiveController.Data;
using TaymotiveModels.Models;

DataGenerator.GenerateUsers();
DataGenerator.GenerateTrainers();
DataGenerator.GenerateVisits();
DataGenerator.GenerateMeasurements();

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


string visitsPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Files",
    "visits.json"
);

FileHandler<Visit> visitHandler =
    new FileHandler<Visit>(visitsPath);

List<Visit> visits = visitHandler.Read();

Console.WriteLine("Visitas registradas: " + visits.Count);

string measurementsPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Files",
    "measurements.json"
);

FileHandler<Measurement> measurementHandler =
    new FileHandler<Measurement>(measurementsPath);

List<Measurement> measurements = measurementHandler.Read();

Console.WriteLine(
    "Mediciones registradas: " + measurements.Count
);

