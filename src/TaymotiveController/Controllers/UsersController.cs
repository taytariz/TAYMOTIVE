
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TaymotiveController.Data;
using TaymotiveController.Security;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class UsersController
    {
        private readonly FileHandler<User> fileHandler;

        public UsersController(string filePath)
        {
            fileHandler = new FileHandler<User>(filePath);
        }

        // Obtener todos los usuarios
        public List<User> GetUsers()
        {
            return fileHandler.Read();
        }

        // Obtener la cantidad total de usuarios
        public int GetTotalUsers()
        {
            return GetUsers().Count;
        }

        // Buscar un usuario por su ID
        public User? GetUserById(int id)
        {
            return GetUsers().FirstOrDefault(
                user => user.Id == id
            );
        }

        // Registrar un nuevo usuario
        public bool RegisterUser(
            string name,
            int age,
            double weight,
            double height,
            string username,
            string password,
            string goal)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                age <= 0 ||
                weight <= 0 ||
                height <= 0 ||
                password.Length < 12)
            {
                return false;
            }

            List<User> users = GetUsers();

            bool exists = users.Any(
                user => string.Equals(
                    user.Username,
                    username.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (exists)
            {
                return false;
            }

            int newId = users.Count == 0
                ? 1
                : users.Max(user => user.Id) + 1;

            User newUser = new User(
                newId,
                name.Trim(),
                age,
                weight,
                height,
                username.Trim(),
                PasswordHelper.HashPassword(password),
                goal.Trim()
            );

            // La cuenta ya tiene contraseña
            newUser.IsActivated = true;

            users.Add(newUser);
            fileHandler.Save(users);

            return true;
        }

        // Modificar un usuario existente
        public bool UpdateUser(
            int id,
            string name,
            int age,
            double weight,
            double height,
            string username,
            string goal)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(username) ||
                age <= 0 ||
                weight <= 0 ||
                height <= 0)
            {
                return false;
            }

            List<User> users = GetUsers();

            User? userToUpdate = users.FirstOrDefault(
                user => user.Id == id
            );

            if (userToUpdate == null)
            {
                return false;
            }

            bool usernameExists = users.Any(
                user => user.Id != id &&
                string.Equals(
                    user.Username,
                    username.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (usernameExists)
            {
                return false;
            }

            userToUpdate.Name = name.Trim();
            userToUpdate.Age = age;
            userToUpdate.Weight = weight;
            userToUpdate.Height = height;
            userToUpdate.Username = username.Trim();
            userToUpdate.Goal = goal.Trim();

            fileHandler.Save(users);

            return true;
        }

        // Generar código de activación
        // Solo puede solicitarlo una cuenta administradora
        public string? GenerateActivationCode(
            int administratorId,
            int userId)
        {
            List<User> users = GetUsers();

            User? administrator = users.FirstOrDefault(
                user => user.Id == administratorId
            );

            if (administrator == null ||
                !administrator.IsAdmin)
            {
                return null;
            }

            User? userToActivate = users.FirstOrDefault(
                user => user.Id == userId
            );

            if (userToActivate == null ||
                userToActivate.IsAdmin ||
                string.Equals(
                    userToActivate.Username,
                    "admin",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                return null;
            }

            // Evitar generar códigos para cuentas
            // que ya tienen contraseña
            if (userToActivate.IsActivated ||
                !string.IsNullOrWhiteSpace(
                    userToActivate.Password
                ))
            {
                return null;
            }

            // Generar un código aleatorio de 12 caracteres
            byte[] randomBytes =
                RandomNumberGenerator.GetBytes(9);

            string activationCode =
                Convert.ToHexString(randomBytes);

            // Guardar solamente el hash del código
            userToActivate.ActivationCodeHash =
                PasswordHelper.HashPassword(
                    activationCode
                );

            // El código vence en 24 horas
            userToActivate.ActivationCodeExpiration =
                DateTime.UtcNow.AddHours(24);

            fileHandler.Save(users);

            // Mostrar el código solamente al generarlo
            return activationCode;
        }

        // Activar cuenta con código y contraseña nueva
        public bool ActivateAccount(
            string username,
            string activationCode,
            string newPassword)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(activationCode) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                newPassword.Length < 12)
            {
                return false;
            }

            List<User> users = GetUsers();

            User? user = users.FirstOrDefault(
                item => string.Equals(
                    item.Username,
                    username.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (user == null ||
                user.IsAdmin ||
                user.IsActivated ||
                !string.IsNullOrWhiteSpace(user.Password))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    user.ActivationCodeHash
                ) ||
                user.ActivationCodeExpiration == null)
            {
                return false;
            }

            // Comprobar vencimiento
            if (DateTime.UtcNow >
                user.ActivationCodeExpiration.Value)
            {
                return false;
            }

            // Verificar el código ingresado
            bool validCode =
                PasswordHelper.VerifyPassword(
                    activationCode.Trim(),
                    user.ActivationCodeHash
                );

            if (!validCode)
            {
                return false;
            }

            // Guardar contraseña protegida
            user.Password =
                PasswordHelper.HashPassword(
                    newPassword
                );

            user.IsActivated = true;

            // Invalidar el código utilizado
            user.ActivationCodeHash = string.Empty;
            user.ActivationCodeExpiration = null;

            fileHandler.Save(users);

            return true;
        }

        // Consultar si un usuario está activado
        public bool IsUserActivated(int userId)
        {
            User? user = GetUserById(userId);

            if (user == null)
            {
                return false;
            }

            // Compatibilidad con cuentas existentes
            return user.IsActivated ||
                !string.IsNullOrWhiteSpace(
                    user.Password
                );
        }

        // Restablecer contraseña
        public bool ResetUserPassword(
            int userId,
            string newPassword)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(newPassword) ||
                newPassword.Length < 12)
            {
                return false;
            }

            List<User> users = GetUsers();

            User? user = users.FirstOrDefault(
                item => item.Id == userId
            );

            if (user == null)
            {
                return false;
            }

            if (user.IsAdmin ||
                string.Equals(
                    user.Username,
                    "admin",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                return false;
            }

            user.Password =
                PasswordHelper.HashPassword(
                    newPassword
                );

            user.IsActivated = true;

            // Invalidar cualquier código pendiente
            user.ActivationCodeHash = string.Empty;
            user.ActivationCodeExpiration = null;

            fileHandler.Save(users);

            return true;
        }

        // Eliminar un usuario existente
        public bool DeleteUser(int id)
        {
            List<User> users = GetUsers();

            User? userToDelete = users.FirstOrDefault(
                user => user.Id == id
            );

            if (userToDelete == null)
            {
                return false;
            }

            // Proteger cuentas administrativas
            if (userToDelete.IsAdmin ||
                string.Equals(
                    userToDelete.Username,
                    "admin",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                return false;
            }

            users.Remove(userToDelete);
            fileHandler.Save(users);

            return true;
        }
    }
}
