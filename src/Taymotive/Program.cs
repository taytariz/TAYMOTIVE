
using Taymotive.Views;

namespace Taymotive
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginView());
        }
    }
}
