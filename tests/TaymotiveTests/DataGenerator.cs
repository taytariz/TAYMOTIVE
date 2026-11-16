
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

        public static void GenerateTrainers()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "trainers.json"
            );

            FileHandler<Trainer> fileHandler =
                new FileHandler<Trainer>(filePath);

            List<Trainer> trainers = new List<Trainer>();

            string[] specialties =
            {
                "Musculacion",
                "CrossFit",
                "Yoga",
                "Spinning",
                "Entrenamiento funcional"
            };

            for (int i = 1; i <= 40; i++)
            {
                Trainer trainer = new Trainer(
                    i,
                    "Entrenador " + i,
                    25 + (i % 25),
                    65 + (i % 25),
                    1.65 + (i % 20) * 0.01,
                    specialties[(i - 1) % specialties.Length],
                    i % 2 == 0
                );

                trainers.Add(trainer);
            }

            fileHandler.Save(trainers);

            Console.WriteLine(
                "Se generaron 40 entrenadores correctamente."
            );
        }

        public static void GenerateVisits()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "visits.json"
            );

            FileHandler<Visit> fileHandler =
                new FileHandler<Visit>(filePath);

            List<Visit> visits = new List<Visit>();

            string[] classes =
            {
                "Yoga",
                "Spinning",
                "CrossFit",
                "Funcional",
                "Musculacion"
            };

            for (int i = 1; i <= 40; i++)
            {
                Visit visit = new Visit(
                    i,
                    i,
                    DateTime.Today.AddDays(-(i % 30)),
                    classes[(i - 1) % classes.Length]
                );

                visits.Add(visit);
            }

            fileHandler.Save(visits);

            Console.WriteLine(
                "Se generaron 40 visitas correctamente."
            );
        }

        public static void GenerateMeasurements()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "measurements.json"
            );

            FileHandler<Measurement> fileHandler =
                new FileHandler<Measurement>(filePath);

            List<Measurement> measurements =
                new List<Measurement>();

            for (int i = 1; i <= 40; i++)
            {
                Measurement measurement = new Measurement(
                    i,
                    i,
                    DateTime.Today.AddDays(-(i % 30)),
                    60 + (i % 30),
                    1.60 + (i % 20) * 0.01
                );

                measurements.Add(measurement);
            }

            fileHandler.Save(measurements);

            Console.WriteLine(
                "Se generaron 40 mediciones correctamente."
            );
        }
    }
}
