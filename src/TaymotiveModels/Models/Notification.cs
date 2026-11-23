
using System;

namespace TaymotiveModels.Models
{
    public class Notification
    {
        public int UserId { get; set; }

        public string Message { get; set; } =
            string.Empty;

        public string Type { get; set; } =
            string.Empty;

        public DateTime Date { get; set; }

        public Notification()
        {
        }

        public Notification(
            int userId,
            string message,
            string type,
            DateTime date)
        {
            UserId = userId;
            Message = message;
            Type = type;
            Date = date;
        }
    }
}
