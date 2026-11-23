
using System;
using System.Collections.Generic;
using System.Linq;
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class BadgesController
    {
        private readonly FileHandler<Badge> fileHandler;

        public BadgesController(string filePath)
        {
            fileHandler = new FileHandler<Badge>(filePath);
        }

        // Obtener todas las insignias
        public List<Badge> GetBadges()
        {
            return fileHandler.Read();
        }

        // Obtener insignias de un usuario
        public List<Badge> GetBadgesByUser(int userId)
        {
            return fileHandler.Read()
                .Where(badge => badge.UserId == userId)
                .OrderByDescending(badge => badge.AwardedDate)
                .ToList();
        }

        // Buscar insignia por ID
        public Badge? GetBadgeById(int id)
        {
            return fileHandler.Read()
                .FirstOrDefault(badge => badge.Id == id);
        }

        // Verificar si el usuario tiene una insignia
        public bool HasBadge(int userId, string badgeName)
        {
            return fileHandler.Read().Any(
                badge =>
                    badge.UserId == userId &&
                    badge.Name.Equals(
                        badgeName,
                        StringComparison.OrdinalIgnoreCase)
            );
        }

        // Otorgar una insignia
        public bool AwardBadge(
            int userId,
            string name,
            string description,
            string badgeType)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(badgeType))
            {
                return false;
            }

            List<Badge> badges = fileHandler.Read();

            // Evitar insignias duplicadas
            bool alreadyExists = badges.Any(
                badge =>
                    badge.UserId == userId &&
                    badge.Name.Equals(
                        name.Trim(),
                        StringComparison.OrdinalIgnoreCase)
            );

            if (alreadyExists)
            {
                return false;
            }

            int newId = badges.Count > 0
                ? badges.Max(badge => badge.Id) + 1
                : 1;

            Badge newBadge = new Badge(
                newId,
                userId,
                name.Trim(),
                description.Trim(),
                badgeType.Trim(),
                DateTime.Now
            );

            badges.Add(newBadge);

            fileHandler.Save(badges);

            return true;
        }

        // Eliminar una insignia
        public bool DeleteBadge(int id)
        {
            List<Badge> badges = fileHandler.Read();

            Badge? badge = badges.FirstOrDefault(
                item => item.Id == id
            );

            if (badge == null)
            {
                return false;
            }

            badges.Remove(badge);

            fileHandler.Save(badges);

            return true;
        }
    }
}
