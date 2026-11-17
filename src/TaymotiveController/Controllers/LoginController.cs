
using TaymotiveController.Data;
using TaymotiveController.Security;
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
                if (string.Equals(
                    user.Username,
                    username,
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(user.Password))
                    {
                        return false;
                    }

                    return PasswordHelper.VerifyPassword(
                        password,
                        user.Password
                    );
                }
            }

            return false;
        }
    }
}
