
using System;
using System.Collections.Generic;
using System.Linq;
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

            // Evitar eliminar la cuenta administradora
            if (string.Equals(
                userToDelete.Username,
                "admin",
                StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            users.Remove(userToDelete);

            fileHandler.Save(users);

            return true;
        }
    }
}
