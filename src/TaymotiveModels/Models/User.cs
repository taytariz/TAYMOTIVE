
using System.Collections.Generic;

namespace TaymotiveModels.Models
{
    public class User : Person
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Goal { get; set; } = string.Empty;

        public List<Routine> Routines { get; set; } = new List<Routine>();

        public User()
        {
        }

        public User(int id, string name, int age,
                    double weight, double height,
                    string username, string password,
                    string goal)
            : base(id, name, age, weight, height)
        {
            Username = username;
            Password = password;
            Goal = goal;
        }
    }
}
