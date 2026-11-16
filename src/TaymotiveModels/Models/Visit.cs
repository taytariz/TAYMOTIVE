
namespace TaymotiveModels.Models
{
    public class Visit
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime VisitDate { get; set; }

        public string ClassName { get; set; } = string.Empty;

        public Visit()
        {
        }

        public Visit(int id, int userId,
                     DateTime visitDate, string className)
        {
            Id = id;
            UserId = userId;
            VisitDate = visitDate;
            ClassName = className;
        }
    }
}

