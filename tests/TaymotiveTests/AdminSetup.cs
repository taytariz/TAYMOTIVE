
using TaymotiveController.Data;
using TaymotiveController.Security;
using TaymotiveModels.Models;

namespace TaymotiveTests
{
    public class AdminSetup
    {
        public static void CreateTestAccount()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            FileHandler<User> fileHandler =
                new FileHandler<User>(filePath);

            List<User> users = fileHandler.Read();

            User? admin = users.FirstOrDefault(
                user => user.Username == "admin"
            );

            if (admin != null)
            {
                Console.WriteLine(
                    "La cuenta de prueba ya existe."
                );
                return;
            }

            Console.Write("Contraseña para admin: ");

            string password = Console.ReadLine() ?? "";

            if (password.Length < 12)
            {
                Console.WriteLine(
                    "La contraseña debe tener al menos 12 caracteres."
                );
                return;
            }

            User newAdmin = new User(
                41,
                "Administrador",
                25,
                70,
                1.75,
                "admin",
                PasswordHelper.HashPassword(password),
                "Administración del gimnasio"
            );

            users.Add(newAdmin);

            fileHandler.Save(users);

            Console.WriteLine(
                "Cuenta de prueba creada correctamente."
            );
        }
    }
}
