
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

        public List<User> GetUsers()
        {
            return fileHandler.Read();
        }

        public int GetTotalUsers()
        {
            return GetUsers().Count;
        }

        public User? GetUserById(int id)
        {
            return GetUsers().FirstOrDefault(
                user => user.Id == id
            );
        }

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
                    username,
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
    }
}
