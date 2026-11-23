
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TaymotiveController.Controllers;

namespace Taymotive.Views
{
    public class ActivateAccountView : Form
    {
        private readonly UsersController usersController;

        private readonly Label lblTitle = new Label();
        private readonly Label lblUsername = new Label();
        private readonly Label lblCode = new Label();
        private readonly Label lblPassword = new Label();
        private readonly Label lblConfirm = new Label();
        private readonly Label lblInfo = new Label();

        private readonly TextBox txtUsername = new TextBox();
        private readonly TextBox txtCode = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly TextBox txtConfirm = new TextBox();

        private readonly Button btnActivate = new Button();
        private readonly Button btnCancel = new Button();

        public ActivateAccountView()
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Files",
                "users.json"
            );

            usersController = new UsersController(filePath);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "TAYMOTIVE - Activar mi cuenta";
            ClientSize = new Size(520, 410);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            lblTitle.Text = "ACTIVAR MI CUENTA";
            lblTitle.Font = new Font(
                "Arial",
                18,
                FontStyle.Bold
            );
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(115, 25);

            lblUsername.Text = "Usuario:";
            lblUsername.Location = new Point(45, 95);
            lblUsername.AutoSize = true;

            txtUsername.Location = new Point(225, 90);
            txtUsername.Width = 245;

            lblCode.Text = "Código de activación:";
            lblCode.Location = new Point(45, 145);
            lblCode.AutoSize = true;

            txtCode.Location = new Point(225, 140);
            txtCode.Width = 245;
            txtCode.MaxLength = 18;

            lblPassword.Text = "Nueva contraseña:";
            lblPassword.Location = new Point(45, 195);
            lblPassword.AutoSize = true;

            txtPassword.Location = new Point(225, 190);
            txtPassword.Width = 245;
            txtPassword.UseSystemPasswordChar = true;

            lblConfirm.Text = "Confirmar contraseña:";
            lblConfirm.Location = new Point(45, 245);
            lblConfirm.AutoSize = true;

            txtConfirm.Location = new Point(225, 240);
            txtConfirm.Width = 245;
            txtConfirm.UseSystemPasswordChar = true;

            lblInfo.Text =
                "La contraseña debe tener al menos 12 caracteres.";

            lblInfo.Location = new Point(95, 285);
            lblInfo.AutoSize = true;
            lblInfo.ForeColor = Color.DimGray;

            btnActivate.Text = "Activar cuenta";
            btnActivate.Location = new Point(115, 325);
            btnActivate.Size = new Size(135, 40);
            btnActivate.Click += BtnActivate_Click;

            btnCancel.Text = "Cancelar";
            btnCancel.Location = new Point(270, 325);
            btnCancel.Size = new Size(135, 40);
            btnCancel.Click += BtnCancel_Click;

            AcceptButton = btnActivate;
            CancelButton = btnCancel;

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblCode);
            Controls.Add(txtCode);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(lblConfirm);
            Controls.Add(txtConfirm);
            Controls.Add(lblInfo);
            Controls.Add(btnActivate);
            Controls.Add(btnCancel);
        }

        // Activar cuenta
        private void BtnActivate_Click(
            object? sender,
            EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string code = txtCode.Text.Trim();
            string password = txtPassword.Text;
            string confirmation = txtConfirm.Text;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmation))
            {
                MessageBox.Show(
                    "Debe completar todos los campos.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (password.Length < 12)
            {
                MessageBox.Show(
                    "La contraseña debe tener " +
                    "al menos 12 caracteres.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (password != confirmation)
            {
                MessageBox.Show(
                    "Las contraseñas no coinciden.",
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                bool activated =
                    usersController.ActivateAccount(
                        username,
                        code,
                        password
                    );

                if (activated)
                {
                    MessageBox.Show(
                        "Cuenta activada correctamente.\n" +
                        "Ahora puede iniciar sesión.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo activar la cuenta.\n" +
                        "Verifique el usuario y el código " +
                        "de activación.",
                        "TAYMOTIVE",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtCode.Clear();
                    txtCode.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al activar cuenta: " +
                    ex.Message,
                    "TAYMOTIVE",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Cancelar activación
        private void BtnCancel_Click(
            object? sender,
            EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
