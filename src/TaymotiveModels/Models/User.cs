
using System;
using System.Collections.Generic;

namespace TaymotiveModels.Models
{
    public class User : Person
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Goal { get; set; } = string.Empty;

        // Identificar cuentas administrativas
        public bool IsAdmin { get; set; } = false;

        // Código de activación protegido
        public string ActivationCodeHash { get; set; } =
            string.Empty;

        // Fecha de vencimiento del código
        public DateTime? ActivationCodeExpiration
        {
            get;
            set;
        }

        // Estado de activación
        public bool IsActivated { get; set; } = false;

        // Rutinas asignadas al usuario
        public List<Routine> Routines { get; set; } =
            new List<Routine>();

        public User()
        {
        }

        public User(
            int id,
            string name,
            int age,
            double weight,
            double height,
            string username,
            string password,
            string goal)
            : base(id, name, age, weight, height)
        {
            Username = username;
            Password = password;
            Goal = goal;

            IsAdmin = false;

            // Las cuentas creadas con contraseña
            // ya tienen sus credenciales configuradas
            IsActivated =
                !string.IsNullOrWhiteSpace(password);

            ActivationCodeHash = string.Empty;
            ActivationCodeExpiration = null;
        }
    }
}
