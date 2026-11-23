
using System;
using System.Collections.Generic;
using System.IO;
using TaymotiveController.Data;
using TaymotiveController.Security;
using TaymotiveModels.Models;

namespace TaymotiveTests
{
    public static class Program
    {
        // Herramienta de verificación manual.
        // No se ejecuta automáticamente con xUnit.
        public static void VerifyProjectData()
        {
            string filesPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files"
            );

            FileHandler<User> userHandler =
                new FileHandler<User>(
                    Path.Combine(filesPath, "users.json")
                );

            List<User> users = userHandler.Read();

            Console.WriteLine(
                "Usuarios registrados: " + users.Count
            );

            int totalRoutines = 0;

            foreach (User user in users)
            {
                totalRoutines += user.Routines.Count;
            }

            Console.WriteLine(
                "Total de rutinas: " + totalRoutines
            );

            FileHandler<Trainer> trainerHandler =
                new FileHandler<Trainer>(
                    Path.Combine(filesPath, "trainers.json")
                );

            Console.WriteLine(
                "Entrenadores registrados: " +
                trainerHandler.Read().Count
            );

            FileHandler<Visit> visitHandler =
                new FileHandler<Visit>(
                    Path.Combine(filesPath, "visits.json")
                );

            Console.WriteLine(
                "Visitas registradas: " +
                visitHandler.Read().Count
            );

            FileHandler<Measurement> measurementHandler =
                new FileHandler<Measurement>(
                    Path.Combine(filesPath, "measurements.json")
                );

            Console.WriteLine(
                "Mediciones registradas: " +
                measurementHandler.Read().Count
            );
        }

        // Verificación manual de contraseñas.
        public static void VerifyPasswordSecurity()
        {
            string testPassword = "PruebaTemporal123!";

            string passwordHash =
                PasswordHelper.HashPassword(testPassword);

            bool validPassword =
                PasswordHelper.VerifyPassword(
                    testPassword,
                    passwordHash
                );

            bool invalidPassword =
                PasswordHelper.VerifyPassword(
                    "ClaveIncorrecta",
                    passwordHash
                );

            Console.WriteLine(
                "Contraseña correcta: " + validPassword
            );

            Console.WriteLine(
                "Contraseña incorrecta: " + invalidPassword
            );
        }

        // La creación de cuentas de prueba
        // permanece en AdminSetup.cs.
        // No se ejecuta automáticamente.
    }
}
