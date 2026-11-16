
using TaymotiveTests;
using TaymotiveController.Data;
using TaymotiveModels.Models;

DataGenerator.GenerateUsers();

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
