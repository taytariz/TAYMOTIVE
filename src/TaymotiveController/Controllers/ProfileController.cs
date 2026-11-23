
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveController.Security;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class ProfileController
    {
        private readonly FileHandler<User> fileHandler;

        public ProfileController(string filePath)
        {
            fileHandler = new FileHandler<User>(filePath);
        }

        // Consultar información del usuario
        public User? GetUserProfile(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }

            return fileHandler.Read()
                .FirstOrDefault(user => user.Id == userId);
        }

        // Cambiar la contraseña de la cuenta autenticada
        public bool ChangePassword(
            User currentUser,
            string currentPassword,
            string newPassword)
        {
            if (currentUser == null ||
                currentUser.Id <= 0 ||
                string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                newPassword.Length < 12)
            {
                return false;
            }

            List<User> users = fileHandler.Read();

            User? user = users.FirstOrDefault(
                item => item.Id == currentUser.Id &&
                string.Equals(
                    item.Username,
                    currentUser.Username,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (user == null ||
                string.IsNullOrWhiteSpace(user.Password))
            {
                return false;
            }

            // Verificar la contraseña actual
            bool validPassword =
                PasswordHelper.VerifyPassword(
                    currentPassword,
                    user.Password
                );

            if (!validPassword)
            {
                return false;
            }

            // Evitar reutilizar la contraseña actual
            if (PasswordHelper.VerifyPassword(
                newPassword,
                user.Password))
            {
                return false;
            }

            // Guardar la nueva contraseña cifrada mediante hash
            user.Password =
                PasswordHelper.HashPassword(newPassword);

            fileHandler.Save(users);

            return true;
        }
    }
}
