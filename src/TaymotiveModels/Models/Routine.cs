
namespace TaymotiveModels.Models
{
    public class Routine
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Routine()
        {
        }

        public Routine(int id, string name, string description,
                       int duration, DateTime startDate, DateTime endDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Duration = duration;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
