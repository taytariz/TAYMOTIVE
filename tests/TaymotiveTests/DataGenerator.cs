
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveTests
{
    public class DataGenerator
    {
        public static void GenerateUsers()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            FileHandler<User> fileHandler =
                new FileHandler<User>(filePath);

            List<User> users = new List<User>();

            for (int i = 1; i <= 40; i++)
            {
                User user = new User(
                    i,
                    "Usuario " + i,
                    18 + (i % 35),
                    60 + (i % 30),
                    1.60 + (i % 20) * 0.01,
                    "usuario" + i,
                    string.Empty,
                    "Mejorar condición física"
                );

                for (int j = 1; j <= 5; j++)
                {
                    Routine routine = new Routine(
                        (i - 1) * 5 + j,
                        "Rutina " + j,
                        "Entrenamiento físico " + j,
                        45,
                        DateTime.Today,
                        DateTime.Today.AddDays(30)
                    );

                    user.Routines.Add(routine);
                }

                users.Add(user);
            }

            fileHandler.Save(users);

            Console.WriteLine(
                "Se generaron 40 usuarios con 5 rutinas cada uno."
            );
        }
    }
}
