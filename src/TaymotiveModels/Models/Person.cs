
namespace TaymotiveModels.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public double Weight { get; set; }

        public double Height { get; set; }

        public Person()
        {
        }

        public Person(int id, string name, int age, double weight, double height)
        {
            Id = id;
            Name = name;
            Age = age;
            Weight = weight;
            Height = height;
        }

        public double CalculateBMI()
        {
            if (Height <= 0)
            {
                return 0;
            }

            return Weight / (Height * Height);
        }
    }
}
