
using System;

namespace TaymotiveModels.Models
{
    public class Badge
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string BadgeType { get; set; } = string.Empty;

        public DateTime AwardedDate { get; set; }

        public Badge()
        {
        }

        public Badge(
            int id,
            int userId,
            string name,
            string description,
            string badgeType,
            DateTime awardedDate)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Description = description;
            BadgeType = badgeType;
            AwardedDate = awardedDate;
        }
    }
}
