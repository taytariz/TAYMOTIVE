
namespace TaymotiveModels.Models
{
    public class Measurement
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime MeasurementDate { get; set; }

        public double Weight { get; set; }

        public double Height { get; set; }

        public double BMI { get; set; }

        public Measurement()
        {
        }

        public Measurement(int id, int userId,
                           DateTime measurementDate,
                           double weight, double height)
        {
            Id = id;
            UserId = userId;
            MeasurementDate = measurementDate;
            Weight = weight;
            Height = height;

            BMI = CalculateBMI();
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
