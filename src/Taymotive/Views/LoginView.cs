
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class LoginView : Form
    {
        private Label lblTitle = new Label();
        private Label lblUsername = new Label();
        private Label lblPassword = new Label();

        private TextBox txtUsername = new TextBox();
        private TextBox txtPassword = new TextBox();

        private Button btnLogin = new Button();

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
            this.Text = "TAYMOTIVE - Inicio de sesión";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

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
            btnLogin.Location = new Point(170, 210);
            btnLogin.Size = new Size(180, 40);

            btnLogin.Click += BtnLogin_Click;

            this.AcceptButton = btnLogin;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
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

            if (validUser)
            {
                MessageBox.Show(
                    "Inicio de sesión exitoso.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtPassword.Clear();

                this.Hide();

                using (MainMenuView mainMenu = new MainMenuView())
                {
                    mainMenu.ShowDialog();
                }

                this.Show();
                txtPassword.Focus();
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
