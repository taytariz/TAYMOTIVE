
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;
using TaymotiveModels.Models;

namespace Taymotive.Views
{
    public class LoginView : Form
    {
        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblPassword = new Label();

        private readonly TextBox txtUsername = new TextBox();
        private readonly TextBox txtPassword = new TextBox();

        private readonly Button btnLogin = new Button();
        private readonly Button btnActivate = new Button();

        private readonly LoginController loginController;

        public LoginView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            loginController = new LoginController(filePath);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Inicio de sesión";
            ClientSize = new Size(450, 350);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "TAYMOTIVE";
            lblTitle.Font = new Font(
                "Arial",
                20,
                FontStyle.Bold
            );
            lblTitle.Location = new Point(125, 30);
            lblTitle.AutoSize = true;

            lblUsername.Text = "Usuario:";
            lblUsername.Location = new Point(70, 100);
            lblUsername.AutoSize = true;

            txtUsername.Location = new Point(170, 95);
            txtUsername.Width = 180;

            lblPassword.Text = "Contraseña:";
            lblPassword.Location = new Point(70, 150);
            lblPassword.AutoSize = true;

            txtPassword.Location = new Point(170, 145);
            txtPassword.Width = 180;
            txtPassword.PasswordChar = '*';

            btnLogin.Text = "Ingresar";
            btnLogin.Location = new Point(170, 205);
            btnLogin.Size = new Size(180, 40);
            btnLogin.Click += BtnLogin_Click;

            btnActivate.Text = "Activar mi cuenta";
            btnActivate.Location = new Point(170, 260);
            btnActivate.Size = new Size(180, 40);
            btnActivate.Click += BtnActivate_Click;

            AcceptButton = btnLogin;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(btnActivate);
        }

        // Abrir ventana de activación
        private void BtnActivate_Click(
            object? sender,
            EventArgs e)
        {
            using (ActivateAccountView activateView =
                new ActivateAccountView())
            {
                if (activateView.ShowDialog(this) ==
                    DialogResult.OK)
                {
                    txtPassword.Clear();
                    txtUsername.Focus();
                }
            }
        }

        // Iniciar sesión
        private void BtnLogin_Click(
            object? sender,
            EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Debe ingresar el usuario y la contraseña.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            bool validUser = loginController.ValidateUser(
                username,
                password
            );

            if (validUser &&
                loginController.CurrentUser != null)
            {
                User currentUser =
                    loginController.CurrentUser;

                MessageBox.Show(
                    "Inicio de sesión exitoso.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtPassword.Clear();

                Hide();

                try
                {
                    using (MainMenuView mainMenu =
                        new MainMenuView(currentUser))
                    {
                        mainMenu.ShowDialog();
                    }
                }
                finally
                {
                    loginController.Logout();

                    Show();
                    txtPassword.Focus();
                }
            }
            else
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}
