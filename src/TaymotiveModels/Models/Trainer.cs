
namespace TaymotiveModels.Models
{
    public class Trainer : Person
    {
        public string Specialty { get; set; } = string.Empty;

        public bool IsPersonalTrainer { get; set; }

        public List<Routine> AssignedRoutines { get; set; } = new List<Routine>();

        public Trainer()
        {
        }

        public Trainer(int id, string name, int age,
                       double weight, double height,
                       string specialty, bool isPersonalTrainer)
            : base(id, name, age, weight, height)
        {
            Specialty = specialty;
            IsPersonalTrainer = isPersonalTrainer;
        }
    }
}
