
using System;
using System.Drawing;
using System.Windows.Forms;

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

        public LoginView()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "TAYMOTIVE - Inicio de sesión";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblTitle.Text = "TAYMOTIVE";
            lblTitle.Font = new Font("Arial", 20, FontStyle.Bold);
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

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Próximamente implementaremos el inicio de sesión.",
                "TAYMOTIVE",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
