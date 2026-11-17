
using TaymotiveController.Data;
using TaymotiveModels.Models;

namespace TaymotiveController.Controllers
{
    public class LoginController
    {
        private readonly FileHandler<User> _fileHandler;

        public LoginController(string filePath)
        {
            _fileHandler = new FileHandler<User>(filePath);
        }

        public bool ValidateUser(
            string username,
            string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            List<User> users = _fileHandler.Read();

            foreach (User user in users)
            {
                if (user.Username == username)
                {
                    // La autenticación con hash se implementará
                    // antes de habilitar el acceso real.
                    return false;
                }
            }

            return false;
        }
    }
}
