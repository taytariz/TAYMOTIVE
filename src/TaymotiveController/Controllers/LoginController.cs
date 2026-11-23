
using System;
using System.Collections.Generic;
using TaymotiveController.Data;
using TaymotiveController.Security;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class LoginController
    {
        private readonly FileHandler<User> _fileHandler;

        // Usuario que inició sesión
        public User? CurrentUser { get; private set; }

        public LoginController(string filePath)
        {
            _fileHandler = new FileHandler<User>(filePath);
        }

        // Validar credenciales del usuario
        public bool ValidateUser(
            string username,
            string password)
        {
            // Limpiar cualquier sesión anterior
            CurrentUser = null;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            List<User> users = _fileHandler.Read();

            foreach (User user in users)
            {
                if (!string.Equals(
                    user.Username,
                    username.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Las cuentas sin contraseña
                // todavía no pueden iniciar sesión
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    return false;
                }

                // Compatibilidad con cuentas existentes
                bool accountEnabled =
                    user.IsActivated ||
                    !string.IsNullOrWhiteSpace(user.Password);

                if (!accountEnabled)
                {
                    return false;
                }

                bool validPassword =
                    PasswordHelper.VerifyPassword(
                        password,
                        user.Password
                    );

                if (!validPassword)
                {
                    return false;
                }

                // Conservar el usuario autenticado
                CurrentUser = user;

                return true;
            }

            return false;
        }

        // Cerrar sesión
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
